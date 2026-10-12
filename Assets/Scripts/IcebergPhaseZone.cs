
using UnityEngine;

public class IcebergPhaseZone : MonoBehaviour
{
    [Header("Iceberg Settings")]
    public Collider2D solidCollider;

    void Awake()
    {
        // Find the solid collider on the parent iceberg if unassigned.
        if (solidCollider == null && transform.parent != null)
        {
            Collider2D[] colliders =
                transform.parent.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D col in colliders)
            {
                if (!col.isTrigger && col.gameObject != gameObject)
                {
                    solidCollider = col;
                    break;
                }
            }
        }

        if (solidCollider == null)
        {
            Debug.LogWarning(
                "No solid iceberg collider found for " + gameObject.name,
                gameObject
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("PHASE ZONE DETECTED: " + other.name, gameObject);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER ENTERED THE ICEBERG ZONE!", gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER EXITED THE ICEBERG ZONE!", gameObject);
        }
    }
}