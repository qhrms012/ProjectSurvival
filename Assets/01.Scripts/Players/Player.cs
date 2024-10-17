using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Scanner scanner;
    public Vector2 playerVector;    // 플레이어의 이동 방향을 저장할 변수
    public float playerSpeed;   // 플레이어 이동 속도
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

    private Rigidbody2D rb;          // 물리 연산에 사용할 Rigidbody2D
    private SpriteRenderer sprite;   // 스프라이트 방향 변경에 사용할 SpriteRenderer
    private Animator animator;       // 애니메이션 제어에 사용할 Animator
    private StateMachine stateMachine;   // 상태를 관리할 상태 머신
    private GameObject character;

    private bool isDead = false;
    private bool isHit = false;

    private void Awake()
    {
        // 컴포넌트 참조 초기화
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        character = this.gameObject;
        // 상태 머신 초기화 및 IdleState로 시작
        stateMachine = new StateMachine();
        stateMachine.SetState(new IdleState(stateMachine, animator));
        hands = GetComponentsInChildren<Hand>(true);
    }

    private void OnEnable()
    {
        playerSpeed *= Character.Speed;
        animator.runtimeAnimatorController = animCon[GameManager.Instance.playerId];
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
        stateMachine.SetState(new HitState(character,stateMachine, animator, rb));
    }

    private void Update()
    {
        if (!GameManager.Instance.isLive)
            return;
        // 매 프레임마다 상태 머신을 업데이트
        stateMachine.Update(playerVector);
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isLive)
            return;
        // 플레이어 스프라이트 방향 처리
        if (playerVector.x != 0)
            sprite.flipX = playerVector.x < 0;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.Instance.isLive || isDead)
            return;

        GameManager.Instance.health -= Time.deltaTime * 10;
        if (!isHit)
        {
            OnHit();
            isHit = true;  // Hit 상태가 발생한 이후에는 반복되지 않도록 플래그 설정
            StartCoroutine(ResetHitAfterDelay(0.5f));  // 0.5초 후에 isHit을 false로 초기화
        }
        if (GameManager.Instance.health <= 0 && !isDead)
        {
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            stateMachine.SetState(new DeadState(stateMachine, animator, gameObject));
            isDead = true;  // Dead 상태가 발생한 이후에는 반복되지 않도록 플래그 설정
            GameManager.Instance.GameOver();
        }
    }

    private IEnumerator ResetHitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // 0.5초 대기
        isHit = false;  // isHit을 다시 false로 설정
    }
}
