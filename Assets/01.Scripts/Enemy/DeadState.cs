using Unity.VisualScripting;
using UnityEngine;

public class DeadState : Istate
{
    private StateMachine stateMachine;
    private Animator animator;
    private GameObject enemy;
    private float deathAnimationDuration = 1f; // ��� �ִϸ��̼� ���� �ð�
    private float elapsedTime;

    public DeadState(StateMachine stateMachine, Animator animator, GameObject enemy)
    {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.enemy = enemy;
    }

    public void Enter()
    {
        // ���� �ִϸ��̼��� ���
        animator.Play("Dead");
        elapsedTime = 0f;
    }

    public void Execute(Vector2 dirVec)
    {
        elapsedTime += Time.deltaTime;

        // Dead �ִϸ��̼��� ��� ���̰� �������� Ȯ��
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Dead(); // �ִϸ��̼��� ������ ��Ȱ��ȭ
        }
    }

    public void Exit()
    {
        // ���°� ���� �� �߰� �۾��� �ʿ��ϸ� ���⿡ �ۼ�
    }

    private void Dead()
    {
        // �� ��Ȱ��ȭ
        enemy.SetActive(false);
    }
}
