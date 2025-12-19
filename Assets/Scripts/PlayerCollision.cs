using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private float maxLife = 1f;
    [SerializeField] GameObject cracked;
    [SerializeField] GameObject gameOverUI;
    private void Awake()
    {
        if (cracked != null)
        {
            cracked.SetActive(false);
        }
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            maxLife -= 0.2f;
        }
    }
    private void Update()
    {
        if (maxLife <= 0)
        {
            if (cracked != null)
            {
                cracked.SetActive(true);
            }
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
        }
    }
}
