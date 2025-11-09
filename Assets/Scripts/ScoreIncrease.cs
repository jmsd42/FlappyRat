using UnityEngine;

public class ScoreIncrease : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ScoreManager.instance.UpdateScore();
        }
    }
}