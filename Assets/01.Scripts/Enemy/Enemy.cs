using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public float speed; // 적의 속도
    public float health;
    public float maxHealth;

    public RuntimeAnimatorController[] animCon;

    public Rigidbody2D target; // 추적할 플레이어 또는 목표
    private Rigidbody2D rigid; // 적의 Rigidbody2D
    private SpriteRenderer sprite; // 스프라이트 제어
    private GameObject character;
    private StateMachine stateMachine; // 상태 머신
    private Animator animator;
    private bool isLive; // 적이 살아 있는지 여부

    Collider2D coll;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        character = this.gameObject;
        // 상태 머신 초기화
        stateMachine = new StateMachine();
        // 게임이 시작되면 바로 플레이어를 추적하는 ChaseState로 전환
        //stateMachine.SetState(new ChaseState(stateMachine, animator, speed, rigid));
    }

    private void Update()
    {
        if (!GameManager.Instance.isLive)
            return;
        // 상태 머신 업데이트
        //if (isLive)
        //{
        Vector2 dirVec = target.position - rigid.position;
            stateMachine.Update(dirVec); // 플레이어 방향을 상태에 전달
        //}
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;
        // 적의 스프라이트 방향 처리
        sprite.flipX = target.position.x > rigid.position.x;
    }

    public void ChasePlayer()
    {
        if (isLive)
        {
            // ChaseState로 전환하여 플레이어 추적 시작
            stateMachine.SetState(new ChaseState(stateMachine, animator, speed, rigid));
        }
        
    }

    public void OnHit()
    {
        // 적이 피격되었을 때 피격 상태로 전환
        stateMachine.SetState(new HitState(character, stateMachine, animator, rigid));
    }

    private void OnEnable()
    {
        target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        sprite.sortingOrder = 2;
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        animator.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        ChasePlayer(); // 적이 활성화되면 바로 플레이어 추적 시작
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        //StartCoroutine(KnockBack());

        if (health > 0)
        {
            OnHit();
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            sprite.sortingOrder = 1;
            // DeadState로 전환
            stateMachine.SetState(new DeadState(stateMachine, animator, gameObject));

            if(GameManager.Instance.isLive)
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.Dead);

            GameManager.Instance.kill++;
            GameManager.Instance.GetExp();

        }
    }
    //private void Dead()
    //{
    //    gameObject.SetActive(false);
    //}
}

