using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float gameTime;
    public float maxGameTime = 2 * 10f;




    public ObjectPool ObjectPool;
    public Player player;

    private void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            
        }
    }
}
