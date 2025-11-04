using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [SerializeField] Collectible[] collectiblePrefabs;
    [SerializeField] Vector3 areaSize = new Vector3(36f, 0f, 36f);
    [SerializeField] float spawnInterval = 1.5f;
    [SerializeField] int maxAlive = 20;

    float timer;
    int aliveCount;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            if (aliveCount < maxAlive) SpawnOne();
        }
    }

    void SpawnOne()
    {
        var prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];
        Vector3 pos = new Vector3(
            Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
            0.5f,
            Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f)
        );

        var obj = Instantiate(prefab, pos, Quaternion.identity);
        aliveCount++;
        var hook = obj.gameObject.AddComponent<DestroyNotifier>();
        hook.onDestroyed += () => aliveCount--;
    }
}

public class DestroyNotifier : MonoBehaviour
{
    public System.Action onDestroyed;
    void OnDestroy() => onDestroyed?.Invoke();
}
