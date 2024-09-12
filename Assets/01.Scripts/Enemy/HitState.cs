using Unity.VisualScripting;
using UnityEngine;

public class HitState : Istate
{
    private StateMachine stateMachine;
    private Animator animator;
    private float knockbackDuration = 0.1f; // �ǰ� �� ��� �ð�
    private float elapsedTime;
    private Vector2 knockbackDirection;
    private Rigidbody2D rigid;

    public HitState(StateMachine stateMachine, Animator animator, Rigidbody2D rigid)
    {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.rigid = rigid;
    }

    public void Enter()
    {
        // �ǰ� �ִϸ��̼� ���
        animator.SetTrigger("Hit");
        elapsedTime = 0f;

        // knockback ó��
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dirVec = rigid.position - (Vector2)playerPos;
        knockbackDirection = dirVec.normalized;
        rigid.AddForce(knockbackDirection * 1, ForceMode2D.Impulse);
    }

    public void Execute(Vector2 dirVec)
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= knockbackDuration)
        {
            // �ǰ� �� �ٽ� ���� ���·� ���ư�
            stateMachine.SetState(new ChaseState(stateMachine, animator, rigid.velocity.magnitude, rigid));
        }
    }

    public void Exit()
    {
        // ���� ���� �� �߰����� �۾��� �ʿ��ϸ� ���⿡ �ۼ�
    }


}
