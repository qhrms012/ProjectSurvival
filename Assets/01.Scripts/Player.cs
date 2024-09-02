using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 Playervector;
    [SerializeField]
    private float PlayerSpeed;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Vector2 vector2 = Playervector.normalized * PlayerSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position +  vector2);
    }

    private void OnMove(InputValue value)
    {
        Playervector = value.Get<Vector2>();
    }

    private void LateUpdate()
    {
        if(Playervector.x != 0)
            sprite.flipX = Playervector.x < 0;
    }


}
