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

    public DroneState currentState = DroneState.Patrolling;

    private Rigidbody2D rb;
    private Vector2 patrolCenter;
    private Vector2 patrolTarget;
    private Transform targetMachine;
    private float retargetTimer;

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

    // ------------------------------
    //  PATROL
    // ------------------------------
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

    // ------------------------------
    //  TARGETING MACHINES
    // ------------------------------
    void FindMachineToAttack()
    {
        GameObject[] machines = GameObject.FindGameObjectsWithTag("RepairTarget");
        Transform best = null;
        float bestDist = Mathf.Infinity;

        foreach (GameObject go in machines)
        {
            var rt = go.GetComponent<RepairTarget2D>();
            if (rt == null)
                continue;

            // Drone prefers machines that are currently FIXED,
            // so it can break them again.
            if (rt.isDamaged)     // already damaged → robots will handle it
                continue;

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

    // ------------------------------
    //  CHASING
    // ------------------------------
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

    // ------------------------------
    //  ATTACK
    // ------------------------------
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
            // Instantly re-damage the machine (or you could call this repeatedly over time)
            rt.Damage();
        }

        // After attack, forget this machine and resume patrol
        targetMachine = null;
        currentState = DroneState.Patrolling;
        ChooseNewPatrolPoint();
    }

    // ------------------------------
    //  VISUALIZE DETECTION RANGE
    // ------------------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}