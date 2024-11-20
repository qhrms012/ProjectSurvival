using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static float Speed
    {
        get
        {
            if (GameManager.Instance.playerId == 0) return 1.1f;
            if (GameManager.Instance.playerId == 4) return 1.3f; // playerId 4인 경우
            return 1f;
        }
    }

    public static float WeaponSpeed
    {
        get
        {
            if (GameManager.Instance.playerId == 1) return 1.1f;
            if (GameManager.Instance.playerId == 4) return 1.3f; // playerId 4인 경우
            return 1f;
        }
    }

    public static float WeaponRate
    {
        get
        {
            if (GameManager.Instance.playerId == 1) return 0.9f;
            if (GameManager.Instance.playerId == 4) return 1f; // playerId 4인 경우
            return 1f;
        }
    }

    public static float Damage
    {
        get
        {
            if (GameManager.Instance.playerId == 2) return 1.2f;
            if (GameManager.Instance.playerId == 4) return 1.3f; // playerId 4인 경우
            return 1f;
        }
    }

    public static int Count
    {
        get
        {
            if (GameManager.Instance.playerId == 3) return 1;
            if (GameManager.Instance.playerId == 4) return 2; // playerId 4인 경우
            return 0;
        }
    }

}
