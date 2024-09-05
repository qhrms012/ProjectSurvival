using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 playerVector;    // �÷��̾��� �̵� ������ ������ ����
    [SerializeField] private float playerSpeed;   // �÷��̾� �̵� �ӵ�

    private Rigidbody2D rb;          // ���� ���꿡 ����� Rigidbody2D
    private SpriteRenderer sprite;   // ��������Ʈ ���� ���濡 ����� SpriteRenderer
    private Animator animator;       // �ִϸ��̼� ��� ����� Animator
    private StateMachine stateMachine;   // ���¸� ������ ���� �ӽ�

    private void Awake()
    {
        // ������Ʈ ���� �ʱ�ȭ
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // ���� �ӽ� �ʱ�ȭ �� IdleState�� ����
        stateMachine = new StateMachine();
        stateMachine.SetState(new IdleState(stateMachine, animator));
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
        //stateMachine.SetState(new HitState(stateMachine, stateMachine.CurrentState, animator));
    }

    private void Update()
    {
        // �� �����Ӹ��� ���� �ӽ��� ������Ʈ
        stateMachine.Update(playerVector);
    }

    private void LateUpdate()
    {
        // �÷��̾� ��������Ʈ ���� ó��
        if (playerVector.x != 0)
            sprite.flipX = playerVector.x < 0;
    }
}
