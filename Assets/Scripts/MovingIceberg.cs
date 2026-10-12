using UnityEngine;

public class MovingIceberg : MonoBehaviour
{
    [Header("Floating Movement")]
    public float moveDistance = 3f;
    public float moveSpeed = 1.5f;

    private Vector2 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float newY = startPosition.y +
            Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        Vector2 newPosition = new Vector2(
            startPosition.x,
            newY
        );

        if (rb != null)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }
    }
}