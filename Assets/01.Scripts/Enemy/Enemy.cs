using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed; // 적의 속도

    public Rigidbody2D target; // 추적할 플레이어 또는 목표
    private Rigidbody2D rigid; // 적의 Rigidbody2D
    private SpriteRenderer sprite; // 스프라이트 제어

    private StateMachine stateMachine; // 상태 머신
    private Animator animator;
    private bool isLive = true; // 적이 살아 있는지 여부

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 상태 머신 초기화
        stateMachine = new StateMachine();
        // 게임이 시작되면 바로 플레이어를 추적하는 ChaseState로 전환
        stateMachine.SetState(new ChaseState(stateMachine, animator, speed, rigid));
    }

    private void Update()
    {
        // 상태 머신 업데이트
        if (isLive)
        {
            Vector2 dirVec = target.position - rigid.position;
            stateMachine.Update(dirVec); // 플레이어 방향을 상태에 전달
        }
    }

    private void LateUpdate()
    {
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
        //stateMachine.SetState(new HitState(stateMachine, stateMachine.CurrentState, animator));
    }
}

