using UnityEngine;

public class MachineSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject repairTargetPrefab;
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
        int safety = 0; // avoid infinite loops

        while (spawned < machineCount && safety < machineCount * 20)
        {
            safety++;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector2 spawnPos = new Vector2(x, y);

            // Check spacing from other machines
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

            // Actually spawn
            GameObject newMachine = Instantiate(repairTargetPrefab, spawnPos, Quaternion.identity);
            newMachine.tag = "RepairTarget"; // ensure tag is set
            spawned++;
        }

        Debug.Log("Spawned " + spawned + " machines.");
    }
}