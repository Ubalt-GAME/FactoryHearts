using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DroneAI2D : MonoBehaviour
{
    public enum DroneState
    {
        Patrolling,
        ChasingMachine,
        Attacking
    }

    [Header("Settings")]
    public float moveSpeed = 4f;
    public float detectRange = 10f;
    public float attackDistance = 0.7f;
    public float retargetInterval = 1.0f;

    [Header("Patrol")]
    public float patrolRadius = 5f;

    private Rigidbody2D rb;
    private Vector2 patrolCenter;
    private Vector2 patrolTarget;
    private Transform targetMachine;
    private float retargetTimer;

    public DroneState currentState = DroneState.Patrolling;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        patrolCenter = transform.position;
        ChooseNewPatrolPoint();
    }

    void Update()
    {
        retargetTimer += Time.deltaTime;

        switch (currentState)
        {
            case DroneState.Patrolling:
                Patrol();
                if (retargetTimer >= retargetInterval)
                {
                    retargetTimer = 0f;
                    FindMachineToAttack();
                }
                break;

            case DroneState.ChasingMachine:
                ChaseMachine();
                break;

            case DroneState.Attacking:
                AttackMachine();
                break;
        }
    }

    void Patrol()
    {
        Vector2 pos = rb.position;
        Vector2 dir = (patrolTarget - pos).normalized;
        rb.MovePosition(pos + dir * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(pos, patrolTarget) < 0.3f)
        {
            ChooseNewPatrolPoint();
        }
    }

    void ChooseNewPatrolPoint()
    {
        patrolTarget = patrolCenter + Random.insideUnitCircle * patrolRadius;
    }

    void FindMachineToAttack()
    {
        GameObject[] machines = GameObject.FindGameObjectsWithTag("RepairTarget");
        Transform best = null;
        float bestDist = Mathf.Infinity;

        foreach (var go in machines)
        {
            var rt = go.GetComponent<RepairTarget2D>();
            if (rt == null) continue;

            // Drone prefers machines that are fixed (so it can ruin progress)
            if (rt.isDamaged) continue; // skip already broken ones

            float d = Vector2.Distance(transform.position, go.transform.position);
            if (d < bestDist && d <= detectRange)
            {
                bestDist = d;
                best = go.transform;
            }
        }

        if (best != null)
        {
            targetMachine = best;
            currentState = DroneState.ChasingMachine;
        }
    }

    void ChaseMachine()
    {
        if (targetMachine == null)
        {
            currentState = DroneState.Patrolling;
            return;
        }

        Vector2 pos = rb.position;
        Vector2 dest = targetMachine.position;
        Vector2 dir = (dest - pos).normalized;
        rb.MovePosition(pos + dir * moveSpeed * Time.deltaTime);

        float dist = Vector2.Distance(pos, dest);
        if (dist <= attackDistance)
        {
            currentState = DroneState.Attacking;
        }
    }

    void AttackMachine()
    {
        if (targetMachine == null)
        {
            currentState = DroneState.Patrolling;
            return;
        }

        var rt = targetMachine.GetComponent<RepairTarget2D>();
        if (rt != null)
        {
            rt.Damage(); // instantly re-damage it
        }

        // After an attack, go back to patrolling and look for another
        targetMachine = null;
        currentState = DroneState.Patrolling;
        ChooseNewPatrolPoint();
    }
}