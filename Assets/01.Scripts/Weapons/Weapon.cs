using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;


    private void Start()
    {
        Init();
    }
    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;


            default:
                break;

        }
    }

    public void Init()
    {
        switch(id) { 
            case 0:
                speed = 150;
                WeaponCount();
                break;


            default:
                break;
                
        }
    }

    private void WeaponCount()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet = GameManager.Instance.ObjectPool.Get(prefabId).transform;
            bullet.parent = transform;
            bullet.GetComponent<Bullet>().Init(damage, -1); // 무한 관통 무기
        }
    }
}
