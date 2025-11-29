using System;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    private SimpleObjectPool pool;

    public static float currentGlobalSpeed = 6f;

    public Color originalColor;
    public Color newColor = new Color(1f, 1f, 1f, 1f);
    public float changeColor = 5f;
    public SpriteRenderer spriteRenderer;

    public float heightRange = 0.45f;
    public float pipeSpeed = 6f;
    public float pipeDestroy = -10f;


    public float lifeTime = 1.5f;
    private float lifeTimer;


    // lineas de chatgpt
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField][Range(0f, 1f)] private float powerUpChance = 0.4f; // 40%
    private bool canSpawnPowerUp = true;

    public PlayerController player;

    public void SetPool(SimpleObjectPool objectPool)
    {
        pool = objectPool;
    }

    void Awake()
    {
        originalColor = spriteRenderer.color; // Guardar solo una vez
    }

    void OnEnable()
    {
        if (GameController.instance.canPlay)
        {
            lifeTimer = 0;
            ResetValues();   // Restaurar colores ANTES de usar el objeto
            SpawnPipe();
        }
    }

    private void Update()
    {
        if (!GameController.instance.canPlay)
            return;

        lifeTimer += Time.deltaTime;

        // Cambio de color por tiempo (ARREGLO: aplicar a TODOS los SpriteRenderer)
        if (lifeTimer >= changeColor)
        {
            SpriteRenderer[] allRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var r in allRenderers)
                r.color = newColor;
        }

        // Tiempo de vida del tubo
        if (lifeTimer > lifeTime)
        {
            ResetValues();
            ReturnToPool();
        }
    }

    //lineas chatgpt
    private void SpawnPipe()
    {
        // Nueva posición aleatoria vertical
        float offsetY = UnityEngine.Random.Range(-heightRange, heightRange);
        transform.position = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);

        // Asignar límite de destrucción
        PipeMovement pm = GetComponentInChildren<PipeMovement>();

        if (pm == null)
        {
            pm = gameObject.AddComponent<PipeMovement>();
        }
        pm.destroyX = pipeDestroy;

        // Spawnear power-up entre los tubos
        if (canSpawnPowerUp
            && UnityEngine.Random.value < powerUpChance
            && player != null
            && player.CanCollectMorePowerUps()
            && PowerUpItem.activePowerUps < 1) // nuevo límite de power-ups en escena
        {
            SpawnPowerUpBetweenPipes(gameObject);
        }
    }
    public class PipeMovement : MonoBehaviour
    {
        public float destroyX;

        private void OnEnable()
        {
            // Siempre usar la velocidad global actual del juego
            // Esto permite que TODO respondan a powerups
        }

        private void Update()
        {
            transform.position += Vector3.left * PipeController.currentGlobalSpeed * Time.deltaTime;

            if (transform.position.x < destroyX)
            {
                PipeController controller = GetComponent<PipeController>();
                controller?.ReturnToPool();
            }
        }
    }
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
        PipeController.currentGlobalSpeed = newSpeed;
    }

    public void AdjustPipeSpeed(float newSpeed, bool resetTimer = false)
    {
        float oldSpeed = PipeController.currentGlobalSpeed;
        float ratio = newSpeed / oldSpeed;

        // Cambiar velocidad global
        PipeController.currentGlobalSpeed = newSpeed;

        // Ajustar vida del tubo para que no queden demasiado juntos o demasiado separados
        lifeTime = Mathf.Clamp(lifeTime / ratio, 0.5f, 3f);

        if (resetTimer)
            lifeTimer = 0f;
        else
            lifeTimer = lifeTimer * (oldSpeed / newSpeed);

        Debug.Log($"[PipeController] Ajuste: oldSpeed={oldSpeed}, newSpeed={newSpeed}");
    }
    void ReturnToPool()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void ResetValues()
    {
        // Restaurar color del sprite principal
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        // Restaurar color de todos los hijos
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
            r.color = originalColor;
    }
}