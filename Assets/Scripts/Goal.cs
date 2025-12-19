using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Make sure it's the egg
        if (!other.CompareTag("Player"))
            return;

        // Call Game Over
        GameManager.Instance.GameOver();
    }
}
