using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    private float timer;
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }
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
                timer += Time.deltaTime;

                if(timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
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
            case 1:


            default:
                count++;
                speed = 0.5f;
                break;
                
        }
    }
    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (id == 0)
            WeaponCount();
            
    }


    private void WeaponCount()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;
            if(index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.ObjectPool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero); // 무한 관통 무기
        }
    }
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            LevelUp(20, 5);
        }
    }

    private void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;


        Transform bullet = GameManager.Instance.ObjectPool.Get(prefabId).transform;
        bullet.position = transform.position;

        float angle = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg - 90f;
        // bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullet.GetComponent<Bullet>().Init(damage, count, dir); 


    }
}
