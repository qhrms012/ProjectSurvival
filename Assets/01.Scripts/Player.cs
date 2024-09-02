using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 플레이어의 이동 방향을 저장할 변수 (Vector2는 2D 벡터)
    private Vector2 Playervector;

    // 플레이어의 이동 속도를 조절할 변수 (Inspector에서 조정 가능)
    [SerializeField]
    private float PlayerSpeed;

    // Rigidbody2D 컴포넌트를 참조할 변수 (물리 연산에 사용)
    private Rigidbody2D rb;

    // SpriteRenderer 컴포넌트를 참조할 변수 (스프라이트의 방향을 조정할 때 사용)
    private SpriteRenderer sprite;

    // 게임 오브젝트가 생성될 때 초기 설정을 하는 함수 (컴포넌트 참조)
    private void Awake()
    {
        // Rigidbody2D 컴포넌트를 가져와서 rb 변수에 저장
        rb = GetComponent<Rigidbody2D>();

        // SpriteRenderer 컴포넌트를 가져와서 sprite 변수에 저장
        sprite = GetComponent<SpriteRenderer>();
    }

    // 물리 연산이 일정 시간 간격으로 실행되는 함수 (주로 물리 기반 이동 처리)
    private void FixedUpdate()
    {
        // Playervector를 정규화(normalized)하여 방향을 유지하고,
        // 속도와 시간(deltaTime)을 곱해 이동 거리를 계산
        Vector2 vector2 = Playervector.normalized * PlayerSpeed * Time.fixedDeltaTime;

        // Rigidbody2D의 위치를 새로운 위치로 이동시킴
        rb.MovePosition(rb.position + vector2);
    }

    // Input System에서 Move 액션이 발생할 때 호출되는 함수 (InputValue를 통해 입력 값 전달)
    private void OnMove(InputValue value)
    {
        // 입력된 값을 Vector2로 변환하여 Playervector에 저장
        Playervector = value.Get<Vector2>();
    }

    // 모든 업데이트 작업이 완료된 후 실행되는 함수 (주로 그래픽적인 작업 처리)
    private void LateUpdate()
    {
        // Playervector의 x 값이 0이 아니면 (즉, 왼쪽이나 오른쪽으로 이동 중이면)
        if (Playervector.x != 0)
            // x 값이 음수면 스프라이트를 좌우 반전 (왼쪽을 향하도록)
            sprite.flipX = Playervector.x < 0;
    }
}
