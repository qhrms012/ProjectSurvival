using Unity.VisualScripting;
using UnityEngine;

public class HitState : Istate
{
    private GameObject character;
    private StateMachine stateMachine;
    private Animator animator;
    private float knockbackDuration = 0.1f; // 피격 후 대기 시간
    private float elapsedTime;
    private Vector2 knockbackDirection;
    private Rigidbody2D rigid;

    public HitState(GameObject character, StateMachine stateMachine, Animator animator, Rigidbody2D rigid)
    {
        this.character = character;
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.rigid = rigid;
    }

    public void Enter()
    {
        // 피격 애니메이션 재생
        animator.SetTrigger("Hit");
        elapsedTime = 0f;

        // knockback 처리
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dirVec = rigid.position - (Vector2)playerPos;
        knockbackDirection = dirVec.normalized;
        rigid.AddForce(knockbackDirection * 1, ForceMode2D.Impulse);
    }

    public void Execute(Vector2 dirVec)
    {
        elapsedTime += Time.deltaTime;

        // character가 null인지 먼저 확인
        if (character == null)
        {
            Debug.LogError("Character reference is null in HitState!");
            return;
        }

        if (character.CompareTag("Player"))
        {
            stateMachine.SetState(new IdleState(stateMachine, animator));
            return;
        }

        if (character.CompareTag("Enemy") && elapsedTime >= knockbackDuration)
        {
            // 피격 후 다시 추적 상태로 돌아감
            stateMachine.SetState(new ChaseState(stateMachine, animator, rigid.velocity.magnitude, rigid));
        }
    }


    public void Exit()
    {
        // 상태 종료 시 추가적인 작업이 필요하면 여기에 작성
    }
}
