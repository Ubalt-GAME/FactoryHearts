using UnityEngine;

public class MachineSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject repairTargetPrefab;

    [Header("Spawn Settings")]
    public int machineCount = 5;

    [Header("Spawn Bounds (inside walls)")]
    public float minX = -13f;
    public float maxX = 13f;
    public float minY = -8f;
    public float maxY = 8f;

    [Header("Spacing")]
    public float minDistanceBetweenMachines = 2f;

    void Start()
    {
        if (repairTargetPrefab == null)
        {
            Debug.LogError("MachineSpawner: No repairTargetPrefab assigned!");
            return;
        }

        SpawnMachines();
    }

    void SpawnMachines()
    {
        int spawned = 0;
        int safety = 0; // safety guard to avoid infinite loops

        while (spawned < machineCount && safety < machineCount * 20)
        {
            safety++;

            // Pick a random position inside the bounds
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector2 spawnPos = new Vector2(x, y);

            // Check for overlap with existing machines
            Collider2D[] hits = Physics2D.OverlapCircleAll(spawnPos, minDistanceBetweenMachines);
            bool tooClose = false;

            foreach (var h in hits)
            {
                if (h.CompareTag("RepairTarget"))
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;

            // Actually spawn the machine
            GameObject obj = Instantiate(repairTargetPrefab, spawnPos, Quaternion.identity);

            // Make sure it has the right tag
            obj.tag = "RepairTarget";

            spawned++;
        }

        Debug.Log($"MachineSpawner: spawned {spawned} machines.");
    }

    void OnDrawGizmos()
    {
        // visualize spawn region in Scene view
        Gizmos.color = Color.red;
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
        Vector3 size   = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}