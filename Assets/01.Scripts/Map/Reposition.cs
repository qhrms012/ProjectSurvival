using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;
    public bool isInArea = false;
    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            isInArea = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || !isInArea)
            return;

        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 myPos = transform.position;
        

        switch (transform.tag)
        {
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;

                //Vector3 playerDir = GameManager.Instance.player.playerVector;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;

                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 42);
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    Vector3 dist = playerPos - myPos;
                    transform.Translate(dist * 20 + 
                        new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f));
                }
                break;
        }
        isInArea = false;  // Area를 벗어난 후 플래그 리셋
    }


}
