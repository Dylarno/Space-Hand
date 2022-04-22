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

    private bool chargingDash;
    private float startedCharge;
    private float dashChargeFactor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = transform.GetChild(0);
        alternatePunch = false;
        chargingDash = false;
    }

    private void FixedUpdate()
    {
        if (chargingDash) return;
        
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (chargingDash)
        {
            var chargingTime = Time.time - startedCharge;

            if (chargingTime > 0f && chargingTime < 1f)
                dashChargeFactor = 2.5f;

            else if(chargingTime > 1f && chargingTime < 2f)
                dashChargeFactor = 5f;

            else if (chargingTime > 3f)
                dashChargeFactor = 7.5f;


        }

        else
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        } 
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (chargingDash) return;
        FlipSprite(moveInput.x);
    }

    private void OnPunch(InputValue value)
    {
        if (chargingDash) return;

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

    private void OnDash(InputValue value)
    {
        var dashValue = value.Get<float>();

        if (dashValue == 1)
        {
            chargingDash = true;
            startedCharge = Time.time;
        }

        else if (dashValue == 0)
        {
            chargingDash = false;

            var dashLocationX = rb.position.x + (moveInput.x * dashChargeFactor);
            var dashLocationY = rb.position.y + (moveInput.y * dashChargeFactor);
            var dashLocation = new Vector2(dashLocationX, dashLocationY);
            rb.position = dashLocation;
        }
    }

    private void FlipSprite(float direction)
    {
        if (direction == 0) return;

        if (direction >= 0.5f || direction <= 0.5f)
            sprite.localScale = new Vector2(1 * Mathf.Sign(direction), 1);
    }
}
