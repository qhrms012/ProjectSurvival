using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public float speed; // ���� �ӵ�
    public float health;
    public float maxHealth;

    public RuntimeAnimatorController[] animCon;

    public Rigidbody2D target; // ������ �÷��̾� �Ǵ� ��ǥ
    private Rigidbody2D rigid; // ���� Rigidbody2D
    private SpriteRenderer sprite; // ��������Ʈ ����

    private StateMachine stateMachine; // ���� �ӽ�
    private Animator animator;
    private bool isLive; // ���� ��� �ִ��� ����

    Collider2D coll;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();

        // ���� �ӽ� �ʱ�ȭ
        stateMachine = new StateMachine();
        // ������ ���۵Ǹ� �ٷ� �÷��̾ �����ϴ� ChaseState�� ��ȯ
        //stateMachine.SetState(new ChaseState(stateMachine, animator, speed, rigid));
    }

    private void Update()
    {
        // ���� �ӽ� ������Ʈ
        //if (isLive)
        //{
            Vector2 dirVec = target.position - rigid.position;
            stateMachine.Update(dirVec); // �÷��̾� ������ ���¿� ����
        //}
    }

    private void LateUpdate()
    {
        // ���� ��������Ʈ ���� ó��
        sprite.flipX = target.position.x > rigid.position.x;
    }

    public void ChasePlayer()
    {
        if (isLive)
        {
            // ChaseState�� ��ȯ�Ͽ� �÷��̾� ���� ����
            stateMachine.SetState(new ChaseState(stateMachine, animator, speed, rigid));
        }
        
    }

    public void OnHit()
    {
        // ���� �ǰݵǾ��� �� �ǰ� ���·� ��ȯ
        stateMachine.SetState(new HitState(stateMachine, animator, rigid));
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
        ChasePlayer(); // ���� Ȱ��ȭ�Ǹ� �ٷ� �÷��̾� ���� ����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet"))
            return;

        health -= collision.GetComponent<Bullet>().damage;
        //StartCoroutine(KnockBack());

        if (health > 0)
        {
            OnHit();
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            sprite.sortingOrder = 1;
            // DeadState�� ��ȯ
            stateMachine.SetState(new DeadState(stateMachine, animator, gameObject));
        }
    }
    //private void Dead()
    //{
    //    gameObject.SetActive(false);
    //}
}

