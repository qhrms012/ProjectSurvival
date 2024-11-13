using UnityEngine;

public class LeaderboardEntry
{
    public string UserEmail { get; set; }
    public float RemainingTime { get; set; }
    public int KillCount { get; set; }
    public Sprite CharacterSprite { get; set; }

    // »ý¼ºÀÚ
    public LeaderboardEntry(string userEmail, float remainingTime, int killCount, Sprite characterSprite)
    {
        UserEmail = userEmail;
        RemainingTime = remainingTime;
        KillCount = killCount;
        CharacterSprite = characterSprite;
    }
}
