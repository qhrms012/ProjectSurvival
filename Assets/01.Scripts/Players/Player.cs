using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Scanner scanner;
    public Vector2 playerVector;    // �÷��̾��� �̵� ������ ������ ����
    public float playerSpeed;   // �÷��̾� �̵� �ӵ�
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

    private Rigidbody2D rb;          // ���� ���꿡 ����� Rigidbody2D
    private SpriteRenderer sprite;   // ��������Ʈ ���� ���濡 ����� SpriteRenderer
    private Animator animator;       // �ִϸ��̼� ��� ����� Animator
    private StateMachine stateMachine;   // ���¸� ������ ���� �ӽ�
    private GameObject character;

    private bool isDead = false;
    private bool isHit = false;

    private void Awake()
    {
        // ������Ʈ ���� �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        character = this.gameObject;
        // ���� �ӽ� �ʱ�ȭ �� IdleState�� ����
        stateMachine = new StateMachine();
        stateMachine.SetState(new IdleState(stateMachine, animator));
        hands = GetComponentsInChildren<Hand>(true);
    }

    private void OnEnable()
    {
        playerSpeed *= Character.Speed;
        animator.runtimeAnimatorController = animCon[GameManager.Instance.playerId];
    }
    private void FixedUpdate()
    {
        // �÷��̾� �̵� ó�� (���¿� ���� �̵� ������ ����� �� ����)
        Vector2 movement = playerVector.normalized * playerSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    public void OnMove(InputValue value)
    {
        // �̵� �Է��� �޾� ���� �ӽſ� ����
        playerVector = value.Get<Vector2>();
    }

    public void OnHit()
    {
        // �÷��̾ �ǰݵǾ��� �� ���¸� HitState�� ��ȯ
        stateMachine.SetState(new HitState(character,stateMachine, animator, rb));
    }

    private void Update()
    {
        if (!GameManager.Instance.isLive)
            return;
        // �� �����Ӹ��� ���� �ӽ��� ������Ʈ
        stateMachine.Update(playerVector);
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;
        // �÷��̾� ��������Ʈ ���� ó��
        if (playerVector.x != 0)
            sprite.flipX = playerVector.x < 0;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.Instance.isLive || isDead)
            return;

        GameManager.Instance.health -= Time.deltaTime * 10;
        if (!isHit)
        {
            OnHit();
            isHit = true;  // Hit ���°� �߻��� ���Ŀ��� �ݺ����� �ʵ��� �÷��� ����
            StartCoroutine(ResetHitAfterDelay(0.5f));  // 0.5�� �Ŀ� isHit�� false�� �ʱ�ȭ
        }
        if (GameManager.Instance.health <= 0 && !isDead)
        {
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            stateMachine.SetState(new DeadState(stateMachine, animator, gameObject));
            isDead = true;  // Dead ���°� �߻��� ���Ŀ��� �ݺ����� �ʵ��� �÷��� ����
            GameManager.Instance.GameOver();
        }
    }

    private IEnumerator ResetHitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // 0.5�� ���
        isHit = false;  // isHit�� �ٽ� false�� ����
    }
}
