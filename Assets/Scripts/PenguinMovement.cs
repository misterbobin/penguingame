
using System.Collections;
using UnityEngine;

public class PenguinMovement : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 10f;
    public float acceleration = 20f;
    public float deceleration = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 360f;
    public float rotationOffset = 0f;

    [Header("Pink Fish Ability")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float boostedSpeed = 16f;
    public float boostDuration = 4f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private Vector2 movementInput;
    private Vector2 facingDirection = Vector2.right;

    private bool isDashing;
    private bool isBoosted;
    private bool isPhasing;

    private Collider2D currentIcebergCollider;
    private IcebergPhaseZone currentPhaseZone;

    private Coroutine boostCoroutine;

    public bool IsDashing => isDashing;
    public bool IsBoosted => isBoosted;
    public bool IsPhasing => isPhasing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        if (rb == null)
            Debug.LogError("PenguinMovement needs a Rigidbody2D.");

        if (playerCollider == null)
            Debug.LogError("PenguinMovement needs a Collider2D.");
    }

    void Update()
    {
        if (isPhasing)
        {
            // Prevent steering while passing through an iceberg.
            movementInput = Vector2.zero;
            return;
        }

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Prevent faster diagonal movement.
        movementInput = movementInput.normalized;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (isPhasing)
        {
            // Keep moving forward until the trigger zone is exited.
            rb.velocity = facingDirection * boostedSpeed;
            return;
        }

        if (isDashing)
        {
            rb.velocity = facingDirection * dashSpeed;
        }
        else
        {
            float currentSpeed = isBoosted
                ? boostedSpeed
                : swimSpeed;

            Vector2 targetVelocity =
                movementInput * currentSpeed;

            float currentAcceleration =
                movementInput != Vector2.zero
                ? acceleration
                : deceleration;

            rb.velocity = Vector2.MoveTowards(
                rb.velocity,
                targetVelocity,
                currentAcceleration * Time.fixedDeltaTime
            );
        }

        // Rotate toward the direction the penguin is moving.
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            facingDirection = rb.velocity.normalized;

            float targetAngle =
                Mathf.Atan2(
                    rb.velocity.y,
                    rb.velocity.x
                ) * Mathf.Rad2Deg + rotationOffset;

            float newAngle = Mathf.MoveTowardsAngle(
                rb.rotation,
                targetAngle,
                rotationSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(newAngle);
        }
    }

    public void ActivatePinkFish()
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }

        boostCoroutine = StartCoroutine(PinkFishBoost());
    }

    private IEnumerator PinkFishBoost()
    {
        isBoosted = true;
        isDashing = true;

        // Dash in the direction the penguin currently faces.
        rb.velocity = facingDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // Keep the speed boost active after the dash.
        yield return new WaitForSeconds(boostDuration);

        isBoosted = false;
        boostCoroutine = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IcebergPhaseZone zone =
            other.GetComponent<IcebergPhaseZone>();

        if (zone == null)
        {
            zone = other.GetComponentInParent<IcebergPhaseZone>();
        }

        if (zone != null && isBoosted)
        {
            BeginPhasing(zone);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IcebergPhaseZone zone =
            other.GetComponent<IcebergPhaseZone>();

        if (zone == null)
        {
            zone = other.GetComponentInParent<IcebergPhaseZone>();
        }

        if (zone != null && zone == currentPhaseZone)
        {
            EndPhasing();
        }
    }

    private void BeginPhasing(IcebergPhaseZone zone)
    {
        if (!isBoosted)
            return;

        if (zone.solidCollider == null || playerCollider == null)
        {
            Debug.LogWarning(
                "Phasing failed: assign the iceberg's solid collider."
            );
            return;
        }

        // Restore collision with any previous iceberg first.
        if (currentIcebergCollider != null)
        {
            Physics2D.IgnoreCollision(
                playerCollider,
                currentIcebergCollider,
                false
            );
        }

        currentPhaseZone = zone;
        currentIcebergCollider = zone.solidCollider;
        isPhasing = true;

        // Keep the direction the penguin was traveling.
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            facingDirection = rb.velocity.normalized;
        }

        // Temporarily disable collision with the iceberg.
        Physics2D.IgnoreCollision(
            playerCollider,
            currentIcebergCollider,
            true
        );

        Debug.Log("PHASING THROUGH: " + currentIcebergCollider.name);
    }

    private void EndPhasing()
    {
        if (playerCollider != null &&
            currentIcebergCollider != null)
        {
            Physics2D.IgnoreCollision(
                playerCollider,
                currentIcebergCollider,
                false
            );
        }

        currentIcebergCollider = null;
        currentPhaseZone = null;
        isPhasing = false;

        Debug.Log("Exited iceberg. Normal movement restored.");
    }

    void OnDisable()
    {
        if (playerCollider != null &&
            currentIcebergCollider != null)
        {
            Physics2D.IgnoreCollision(
                playerCollider,
                currentIcebergCollider,
                false
            );
        }
    }
}