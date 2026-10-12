
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    private bool finished;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (finished || !other.CompareTag("Player"))
            return;

        finished = true;

        GameMenuManager menu =
            FindObjectOfType<GameMenuManager>();

        if (menu != null)
        {
            menu.ShowWin();
        }
        else
        {
            Debug.LogWarning("GameMenuManager was not found.");
        }
    }
}