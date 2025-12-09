using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [Header("Drone Settings")]
    public GameObject dronePrefab;
    public int maxDrones = 3;

    [Header("Spawn Timing")]
    public float spawnInterval = 10f;
    private float timer = 0f;

    [Header("Spawn Area (optional)")]
    public float minX = -25;
    public float maxX = 25;
    public float minY = -15;
    public float maxY = 15;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawnDrone();
        }
    }

    void TrySpawnDrone()
    {
        int current = FindObjectsOfType<DroneAI2D>().Length;

        // Do not spawn too many drones
        if (current >= maxDrones)
            return;

        // Pick a random spawn position inside allowed area
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        Vector2 spawnPos = new Vector2(x, y);

        Instantiate(dronePrefab, spawnPos, Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        // visualize the spawn region
        Gizmos.color = Color.magenta;
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}