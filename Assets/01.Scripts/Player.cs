using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // �÷��̾��� �̵� ������ ������ ���� (Vector2�� 2D ����)
    private Vector2 Playervector;

    // �÷��̾��� �̵� �ӵ��� ������ ���� (Inspector���� ���� ����)
    [SerializeField]
    private float PlayerSpeed;

    // Rigidbody2D ������Ʈ�� ������ ���� (���� ���꿡 ���)
    private Rigidbody2D rb;

    // SpriteRenderer ������Ʈ�� ������ ���� (��������Ʈ�� ������ ������ �� ���)
    private SpriteRenderer sprite;

    // ���� ������Ʈ�� ������ �� �ʱ� ������ �ϴ� �Լ� (������Ʈ ����)
    private void Awake()
    {
        // Rigidbody2D ������Ʈ�� �����ͼ� rb ������ ����
        rb = GetComponent<Rigidbody2D>();

        // SpriteRenderer ������Ʈ�� �����ͼ� sprite ������ ����
        sprite = GetComponent<SpriteRenderer>();
    }

    // ���� ������ ���� �ð� �������� ����Ǵ� �Լ� (�ַ� ���� ��� �̵� ó��)
    private void FixedUpdate()
    {
        // Playervector�� ����ȭ(normalized)�Ͽ� ������ �����ϰ�,
        // �ӵ��� �ð�(deltaTime)�� ���� �̵� �Ÿ��� ���
        Vector2 vector2 = Playervector.normalized * PlayerSpeed * Time.fixedDeltaTime;

        // Rigidbody2D�� ��ġ�� ���ο� ��ġ�� �̵���Ŵ
        rb.MovePosition(rb.position + vector2);
    }

    // Input System���� Move �׼��� �߻��� �� ȣ��Ǵ� �Լ� (InputValue�� ���� �Է� �� ����)
    private void OnMove(InputValue value)
    {
        // �Էµ� ���� Vector2�� ��ȯ�Ͽ� Playervector�� ����
        Playervector = value.Get<Vector2>();
    }

    // ��� ������Ʈ �۾��� �Ϸ�� �� ����Ǵ� �Լ� (�ַ� �׷������� �۾� ó��)
    private void LateUpdate()
    {
        // Playervector�� x ���� 0�� �ƴϸ� (��, �����̳� ���������� �̵� ���̸�)
        if (Playervector.x != 0)
            // x ���� ������ ��������Ʈ�� �¿� ���� (������ ���ϵ���)
            sprite.flipX = Playervector.x < 0;
    }
}
