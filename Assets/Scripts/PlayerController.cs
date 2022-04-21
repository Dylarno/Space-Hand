using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    private Animator animator;
    private Transform sprite;

    private bool alternatePunch;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = transform.GetChild(0);
        alternatePunch = false;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        FlipSprite(moveInput.x);
    }

    private void OnPunch(InputValue value)
    {
        var punchInput = value.Get<float>();

        if (punchInput == 0f) return;

        FlipSprite(punchInput);

        if (alternatePunch)
        {
            animator.Play("Punch Left");
            alternatePunch = false;
        }

        else
        {
            animator.Play("Punch Right");
            alternatePunch = true;
        } 
    }

    private void FlipSprite(float direction)
    {
        if (direction == 0) return;

        if (direction >= 0.5f || direction <= 0.5f)
            sprite.localScale = new Vector2(1 * Mathf.Sign(direction), 1);
    }
}
