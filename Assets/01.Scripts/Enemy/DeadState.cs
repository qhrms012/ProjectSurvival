using Unity.VisualScripting;
using UnityEngine;

public class DeadState : Istate
{
    private StateMachine stateMachine;
    private Animator animator;
    private GameObject enemy;
    private float deathAnimationDuration = 1f; // 사망 애니메이션 지속 시간
    private float elapsedTime;

    public DeadState(StateMachine stateMachine, Animator animator, GameObject enemy)
    {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.enemy = enemy;
    }

    public void Enter()
    {
        // 죽음 애니메이션을 재생
        animator.Play("Dead");
        elapsedTime = 0f;
    }

    public void Execute(Vector2 dirVec)
    {
        elapsedTime += Time.deltaTime;

        // Dead 애니메이션이 재생 중이고 끝났는지 확인
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Dead(); // 애니메이션이 끝나면 비활성화
        }
    }

    public void Exit()
    {
        // 상태가 끝날 때 추가 작업이 필요하면 여기에 작성
    }

    private void Dead()
    {
        // 적 비활성화
        enemy.SetActive(false);
    }
}
