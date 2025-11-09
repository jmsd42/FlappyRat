using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("PERDISTE CABEZA DE RATA");
            GameController.instance.CallGameOver();
        }
    }
}
