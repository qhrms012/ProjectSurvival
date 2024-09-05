using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 playerVector;    // 플레이어의 이동 방향을 저장할 변수
    [SerializeField] private float playerSpeed;   // 플레이어 이동 속도

    private Rigidbody2D rb;          // 물리 연산에 사용할 Rigidbody2D
    private SpriteRenderer sprite;   // 스프라이트 방향 변경에 사용할 SpriteRenderer
    private Animator animator;       // 애니메이션 제어에 사용할 Animator
    private StateMachine stateMachine;   // 상태를 관리할 상태 머신

    private void Awake()
    {
        // 컴포넌트 참조 초기화
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 상태 머신 초기화 및 IdleState로 시작
        stateMachine = new StateMachine();
        stateMachine.SetState(new IdleState(stateMachine, animator));
    }

    private void FixedUpdate()
    {
        // 플레이어 이동 처리 (상태에 따라 이동 로직이 변경될 수 있음)
        Vector2 movement = playerVector.normalized * playerSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    public void OnMove(InputValue value)
    {
        // 이동 입력을 받아 상태 머신에 전달
        playerVector = value.Get<Vector2>();
    }

    public void OnHit()
    {
        // 플레이어가 피격되었을 때 상태를 HitState로 전환
        //stateMachine.SetState(new HitState(stateMachine, stateMachine.CurrentState, animator));
    }

    private void Update()
    {
        // 매 프레임마다 상태 머신을 업데이트
        stateMachine.Update(playerVector);
    }

    private void LateUpdate()
    {
        // 플레이어 스프라이트 방향 처리
        if (playerVector.x != 0)
            sprite.flipX = playerVector.x < 0;
    }
}
