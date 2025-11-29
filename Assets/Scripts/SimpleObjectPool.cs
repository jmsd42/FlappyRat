using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject objectToPool;
    public int poolSize = 10;
    public float spawnInterval = 3f;

    private Queue<GameObject> objectPool;
    private float timer;

    void Start()
    {
        InitializePool();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnFromPool();
            timer = 0f;
        }
    }

    void InitializePool()
    {
        objectPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    void SpawnFromPool()
    {
        if (objectPool.Count == 0)
        {
            RefillPool();
        }

        GameObject objectToSpawn = objectPool.Dequeue();
        objectToSpawn.GetComponent<PipeController>()?.SetPool(this);

        objectToSpawn.transform.position = transform.position;
        objectToSpawn.SetActive(true);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    void RefillPool()
    {
        Debug.Log("Refilling pool...");

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }
}