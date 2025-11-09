using System;
using UnityEngine;

public class PipeController : MonoBehaviour
{

    public float maxTime = 1.5f;
    public float heightRange = 0.45f;
    public GameObject pipe;
    public float pipeSpeed = 6f;
    public float pipeDestroy = -10f;

    public float timer;

    // lineas de chatgpt
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField][Range(0f, 1f)] private float powerUpChance = 0.4f; // 40%
    private bool canSpawnPowerUp = true;

    public PlayerController player;

    private void Start()
    {
        if (GameController.instance.canPlay)
        {
            SpawnPipe();
        }
    }

    private void Update()
    {
        if (GameController.instance.canPlay)
        {
            if (timer > maxTime)
            {
                SpawnPipe();
                timer = 0;
            }

            timer += Time.deltaTime;
        }
    }

    //lineas chatgpt
    private void SpawnPipe()
    {
        Vector3 spawnPos = transform.position + new Vector3(0, UnityEngine.Random.Range(-heightRange, heightRange));
        GameObject newPipe = Instantiate(pipe, spawnPos, Quaternion.identity);

        // Añadimos un componente temporal para mover el tubo
        PipeMovement pipeMovement = newPipe.AddComponent<PipeMovement>();
        pipeMovement.speed = pipeSpeed;
        pipeMovement.destroyX = pipeDestroy;

        // Spawnear power-up entre los tubos
        if (canSpawnPowerUp
            && UnityEngine.Random.value < powerUpChance
            && player != null
            && player.CanCollectMorePowerUps()
            && PowerUpItem.activePowerUps < 1) // nuevo límite de power-ups en escena
        {
            SpawnPowerUpBetweenPipes(newPipe);
        }
    }

    private class PipeMovement : MonoBehaviour
    {
        public float speed;
        public float destroyX;

        private void Update()
        {
            transform.position += Vector3.left * speed * Time.deltaTime;

            if (transform.position.x < destroyX)
            {
                Destroy(gameObject);
            }
        }

    }
    //private void SpawnPipe()
    //{
    //    Vector3 spawnPos = transform.position + new Vector3(0, UnityEngine.Random.Range(-heightRange, heightRange));
    //    GameObject newPipe = Instantiate(pipe, spawnPos, Quaternion.identity);

    //    if (GameController.instance.canPlay)
    //    {
    //        Vector3 targetPos = new Vector3(pipeDestroy, newPipe.transform.position.y, newPipe.transform.position.z);
    //        LeanTween.move(newPipe, targetPos, pipeSpeed).setEaseLinear().setOnComplete(() => Destroy(newPipe));
    //    }
    //    else
    //    {
    //        LeanTween.cancel(newPipe);
    //    }

    //    // lineas de chatgpt
    //    if (canSpawnPowerUp && UnityEngine.Random.value < powerUpChance)
    //    {
    //        SpawnPowerUpBetweenPipes(newPipe);
    //    }

    //}

    // lineas de chatgpt
    private void SpawnPowerUpBetweenPipes(GameObject pipeInstance)
    {
        // Busca los tubos dentro del prefab
        Transform topPipe = pipeInstance.transform.Find("DownwardPipe_0");
        Transform bottomPipe = pipeInstance.transform.Find("UpwardPipe_0");

        if (topPipe == null || bottomPipe == null)
        {
            Debug.LogWarning("No se encontraron los tubos dentro del prefab. Revisa los nombres.");
            return;
        }

        // Calculamos el punto medio entre ambos tubos
        float middleY = (topPipe.position.y + bottomPipe.position.y) / 2f;
        Vector3 middlePos = new Vector3(pipeInstance.transform.position.x, middleY, 0f);

        // Instanciamos el power-up como hijo del prefab (para que se mueva junto)
        GameObject powerUp = Instantiate(powerUpPrefab, middlePos, Quaternion.identity, pipeInstance.transform);

        // Ajusta un poco la posición si queda fuera del hueco
        // powerUp.transform.localPosition += new Vector3(0, 0.2f, 0);
    }

    public void UpdateAllPipeSpeeds(float newSpeed)
    {
        pipeSpeed = newSpeed;

        // Busca todos los objetos con el componente PipeMovement y actualiza su velocidad
        PipeMovement[] allPipes = FindObjectsOfType<PipeMovement>();
        foreach (PipeMovement pipe in allPipes)
        {
            pipe.speed = newSpeed;
        }
    }

    public void AdjustPipeSpeed(float newSpeed, bool resetTimer = false)
    {
        float oldSpeed = pipeSpeed;
        float ratio = newSpeed / oldSpeed;

        pipeSpeed = newSpeed;

        // Mantener un ritmo de aparición proporcional a la velocidad
        maxTime = Mathf.Clamp(maxTime / ratio, 0.5f, 3f);

        // En lugar de reiniciar siempre el timer, lo ajustamos proporcionalmente
        timer = timer * (oldSpeed / newSpeed);

        // Asegurar que los tubos actuales también cambian su velocidad
        PipeMovement[] allPipes = FindObjectsOfType<PipeMovement>();
        foreach (PipeMovement pipe in allPipes)
        {
            pipe.speed = newSpeed;
        }

        Debug.Log($"[PipeController] Ajuste: oldSpeed={oldSpeed}, newSpeed={newSpeed}, maxTime={maxTime:F2}, timer={timer:F2}");
    }
}