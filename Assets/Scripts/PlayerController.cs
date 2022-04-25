using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private static float maxDashCharge = 1.5f;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private bool flippedSprite;

    private Transform sprite;
    private SpriteRenderer dashChargeSprite;
    private Animator animator;

    [SerializeField] private GameObject dashTrail;

    private bool alternatePunch;

    private bool chargingDash;
    private float startedCharge;
    private float dashChargeFactor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = transform.GetChild(0);
        dashChargeSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();

        alternatePunch = false;
        chargingDash = false;
        flippedSprite = false;
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

            dashChargeFactor = Mathf.Clamp(chargingTime, maxDashCharge - 1.0f, maxDashCharge);
            dashChargeFactor *= 2.5f;
            print(dashChargeFactor);

            var spriteAlpha = Mathf.Clamp((Map(chargingTime, 0.0f, maxDashCharge, 0.0f, 1.0f)), 0.0f, maxDashCharge);
            dashChargeSprite.color = new Color(1f, 1f, 1f, spriteAlpha);

            animator.SetBool("Charging", true);
        }

        else
        {
            animator.SetBool("Charging", false);
            animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
            dashChargeSprite.color = new Color(1f, 1f, 1f, 0f);
        } 
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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

        Vector2 pos = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 dir = !flippedSprite ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(pos, dir);

        if (hit.collider == null) return;

        if (hit.collider.CompareTag("Asteroid") && Vector2.Distance(transform.position, hit.collider.transform.position) < 1f)
        {
            Destroy(hit.collider.gameObject);
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

            // Center Sprite on Character:
            var dashTrailPosition = new Vector2(transform.position.x, transform.position.y + 0.5f);

            // Map the movement input to Euler angles:
            var dashTrailRotation = Map(Mathf.Round(moveInput.y * 2f) / 2f, -1f, 1f, -90f, 90f);

            // Evaluate for diagonal dashes in the negative x direction and adjust their rotation:
            dashTrailRotation = (moveInput.x < 0f && moveInput.y != 0f) ? dashTrailRotation * Mathf.Sign(moveInput.x) : dashTrailRotation;

            var spawnedTrail = Instantiate(dashTrail, dashTrailPosition, Quaternion.Euler(0f, 0f, dashTrailRotation));

            // Set the sprite's length and flip the sprite based on movement input:
            spawnedTrail.transform.localScale = new Vector3((dashChargeFactor - 0.5f) * Mathf.Sign(moveInput.x), 1f, 2f);

            Destroy(spawnedTrail, 1.0f);

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

        flippedSprite = sprite.localScale.x == -1 ? true : false;
    }

    public float Map(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }
}