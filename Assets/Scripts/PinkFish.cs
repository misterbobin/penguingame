
using UnityEngine;

public class PinkFish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PenguinMovement penguin =
            other.GetComponent<PenguinMovement>();

        if (penguin != null)
        {
            penguin.ActivatePinkFish();
            Destroy(gameObject);
        }
    }
}