using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Firebase.Storage;
using UnityEngine.Networking;
public class DatabaseManager : Singleton<DatabaseManager>
{
    private FirebaseStorage storage;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);


        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        storage = FirebaseStorage.DefaultInstance;
    }
    public async Task<string> UploadSpriteToStorage(Sprite sprite, string userId)
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite is null.");
            return null;
        }

        // Sprite�� Texture2D�� ��ȯ
        Texture2D texture = sprite.texture;
        byte[] spriteBytes = texture.EncodeToPNG(); // PNG �������� ��ȯ

        // Storage ��� ����
        var storageReference = storage.GetReference($"CharacterSprites/{userId}.png");

        // Storage�� ���� ���ε�
        try
        {
            await storageReference.PutBytesAsync(spriteBytes);
            Debug.Log("Sprite uploaded successfully!");

            // ���ε�� ������ �ٿ�ε� URL ��������
            string downloadUrl = (await storageReference.GetDownloadUrlAsync()).ToString();
            return downloadUrl;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to upload sprite to Firebase Storage: " + ex.Message);
            return null;
        }
    }

    public string GetUserId()
    {
        if (auth.CurrentUser != null)
        {
            return auth.CurrentUser.UserId;
        }
        Debug.LogWarning("User is not authenticated.");
        return null;
    }

    public Task SaveLeaderboardEntry(string userId, string userEmail, float remainingTime, int killCount, string characterSpriteUrl)
    {
        var leaderboardEntry = new Dictionary<string, object>
    {
        { "userEmail", userEmail },
        { "remainingTime", remainingTime },
        { "killCount", killCount },
        { "characterSpriteUrl", characterSpriteUrl }
    };
        return databaseReference.Child("leaderboard").Child(userId).Push().SetValueAsync(leaderboardEntry);
    }
    // �̹��� URL�� ���� Sprite �ҷ�����
    private async Task<Sprite> LoadSpriteFromUrl(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load image from URL: {url} Error: {request.error}");
                return null;
            }

            // Texture�� Sprite�� ��ȯ
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    // �������� �����͸� Firebase���� �ҷ����� �޼���
    public async Task<List<Tuple<string, float, int, Sprite>>> LoadLeaderboardEntries()
{
    if (databaseReference == null)
    {
        Debug.LogError("Database reference is null. Make sure InitializeFirebase() is called.");
        return null;
    }

    DataSnapshot snapshot;
    try
    {
        snapshot = await databaseReference.Child("leaderboard").GetValueAsync();
    }
    catch (Exception ex)
    {
        Debug.LogError("Failed to load leaderboard entries: " + ex.Message);
        return null;
    }

    List<Tuple<string, float, int, Sprite>> leaderboardData = new List<Tuple<string, float, int, Sprite>>();

    foreach (var userSnapshot in snapshot.Children)
    {
        foreach (var entrySnapshot in userSnapshot.Children)
        {
            string userEmail = entrySnapshot.Child("userEmail").Value?.ToString() ?? "Unknown Player";
            float remainingTime = float.TryParse(entrySnapshot.Child("remainingTime").Value?.ToString(), out var rt) ? rt : 0f;
            int killCount = int.TryParse(entrySnapshot.Child("killCount").Value?.ToString(), out var kc) ? kc : 0;

            // �̹��� URL ��������
            string characterSpriteUrl = entrySnapshot.Child("characterSpriteUrl").Value?.ToString();
            Sprite characterSprite = null;

            if (!string.IsNullOrEmpty(characterSpriteUrl))
            {
                characterSprite = await LoadSpriteFromUrl(characterSpriteUrl);
            }

            leaderboardData.Add(new Tuple<string, float, int, Sprite>(userEmail, remainingTime, killCount, characterSprite));
        }
    }

    return leaderboardData;
}
    // Ư�� ����� �̸��Ϸ� ���͸��� �������� �����͸� �ҷ����� �޼���
    public async Task<List<Tuple<string, float, int, Sprite>>> LoadUserLeaderboardEntries(string userEmail)
    {
        if (databaseReference == null)
        {
            Debug.LogError("Database reference is null. Make sure InitializeFirebase() is called.");
            return null;
        }

        DataSnapshot snapshot;
        try
        {
            snapshot = await databaseReference.Child("leaderboard").GetValueAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load user leaderboard entries: " + ex.Message);
            return null;
        }

        List<Tuple<string, float, int, Sprite>> userLeaderboardData = new List<Tuple<string, float, int, Sprite>>();

        foreach (var userSnapshot in snapshot.Children)
        {
            foreach (var entrySnapshot in userSnapshot.Children)
            {
                string entryEmail = entrySnapshot.Child("userEmail").Value != null
                    ? entrySnapshot.Child("userEmail").Value.ToString()
                    : "";

                if (entryEmail == userEmail)
                {
                    float remainingTime = entrySnapshot.Child("remainingTime").Value != null
                        ? float.Parse(entrySnapshot.Child("remainingTime").Value.ToString())
                        : 0f;

                    int killCount = entrySnapshot.Child("killCount").Value != null
                        ? int.Parse(entrySnapshot.Child("killCount").Value.ToString())
                        : 0;

                    Sprite characterSprite = null;
                    string spritePath = "CharacterSprites/DefaultSprite";

                    var characterSpriteValue = entrySnapshot.Child("characterSprite").Value;
                    if (characterSpriteValue != null && characterSpriteValue.ToString() != "")
                    {
                        spritePath = "CharacterSprites/" + characterSpriteValue.ToString();
                    }

                    characterSprite = Resources.Load<Sprite>(spritePath);
                    if (characterSprite == null)
                    {
                        Debug.LogWarning($"Character sprite not found at path: {spritePath}");
                    }

                    userLeaderboardData.Add(new Tuple<string, float, int, Sprite>(entryEmail, remainingTime, killCount, characterSprite));
                }
            }
        }

        return userLeaderboardData;
    }
}
