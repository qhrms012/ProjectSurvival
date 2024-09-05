using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : Istate
{
    private StateMachine stateMachine;
    private Animator animator;
    public RunState(StateMachine machine, Animator animator)
    {
        stateMachine = machine;
        this.animator = animator;
    }
    public void Enter()
    {
        Debug.Log("�ٴ»���");
        animator.Play("Run");
    }

    public void Execute(Vector2 playerVector)
    {
        if(playerVector.magnitude == 0)
        {
            stateMachine.SetState(new IdleState(stateMachine, animator));
        }
    }

    public void Exit()
    {
        Debug.Log("�ٴ»��� ����");
    }
}
