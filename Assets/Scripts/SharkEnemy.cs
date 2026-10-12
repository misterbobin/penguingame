
using UnityEngine;

public class SharkEnemy : MonoBehaviour
{
    [Header("Chasing")]
    public float chaseRange = 8f;
    public float moveSpeed = 4f;
    public float rotationSpeed = 180f;
    public float rotationOffset = 0f;

    [Header("Health")]
    public int maxHealth = 100;
    public int boostedDamage = 100;
    public int contactDamage = 20;
    public float damageCooldown = 1f;

    private int currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private Collider2D sharkCollider;
    private Collider2D playerCollider;
    private PenguinMovement penguin;
    private PenguinHealth playerHealth;
    private float nextDamageTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sharkCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerCollider = playerObject.GetComponent<Collider2D>();
            penguin = playerObject.GetComponent<PenguinMovement>();
            playerHealth = playerObject.GetComponent<PenguinHealth>();
        }
        else
        {
            Debug.LogError("SharkEnemy: No GameObject tagged Player found.");
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 difference = (Vector2)player.position - rb.position;
        float distance = difference.magnitude;

        if (distance <= chaseRange)
        {
            Vector2 direction = difference.normalized;
            rb.velocity = direction * moveSpeed;

            float angle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
                + rotationOffset;

            rb.MoveRotation(Mathf.MoveTowardsAngle(
                rb.rotation,
                angle,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        // Backup contact check, even if collision callbacks fail.
        if (playerCollider != null && sharkCollider != null &&
            sharkCollider.bounds.Intersects(playerCollider.bounds))
        {
            HandlePlayerContact();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            HandlePlayerContact();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            HandlePlayerContact();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            HandlePlayerContact();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            HandlePlayerContact();
    }

    void HandlePlayerContact()
    {
        if (penguin != null && penguin.IsBoosted)
        {
            TakeDamage(boostedDamage);
            return;
        }

        if (Time.time >= nextDamageTime && playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log("Shark health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
