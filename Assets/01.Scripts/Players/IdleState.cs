using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : Istate
{
    private StateMachine stateMachine;
    private Animator animator;

    public IdleState(StateMachine machine, Animator animator)
    {
        stateMachine = machine;
        this.animator = animator;
    }
    public void Enter()
    {
        Debug.Log("대기상태");
        animator.Play("Idle");
    }

    public void Execute(Vector2 playerVector)
    {
        if(playerVector.magnitude > 0)
        {
            stateMachine.SetState(new RunState(stateMachine, animator)); // 상태전환
        }
    }

    public void Exit()
    {
        Debug.Log("대기 상태 종료");
    }
}
