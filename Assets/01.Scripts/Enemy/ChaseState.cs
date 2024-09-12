using Unity.VisualScripting;
using UnityEngine;

public class ChaseState : Istate
{
    private StateMachine stateMachine;
    private Animator animator;
    private float speed;
    private Rigidbody2D rigid;

    public ChaseState(StateMachine machine, Animator animator, float speed, Rigidbody2D rigid)
    {
        stateMachine = machine;
        this.animator = animator;
        this.speed = speed;
        this.rigid = rigid;
    }

    public void Enter()
    {
        animator.Play("Run"); // 추적 애니메이션
    }

    public void Execute(Vector2 direction)
    {
       
        // 플레이어를 추적하는 로직
        Vector2 nextVec = direction.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;

    }

    public void Exit()
    {
        Debug.Log("Exit Chase State");
    }
}
