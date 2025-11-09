using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public float lifetime = 6f; // Tiempo antes de desaparecer si no se recoge
    private bool collected = false;
    public static int activePowerUps = 0;

    private void Start()
    {
        activePowerUps++;
        // Destruirlo automáticamente si no se recoge
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;

            // Le avisamos al PlayerController que recoja un power-up
            collision.GetComponent<PlayerController>()?.CollectPowerUp();

            Destroy(gameObject); // Destruye el objeto visual
        }
    }

    private void OnDestroy()
    {
        activePowerUps = Mathf.Max(0, activePowerUps - 1);
    }
}
