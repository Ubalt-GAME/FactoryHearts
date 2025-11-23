using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotAI2D : MonoBehaviour
{
    public enum State
    {
        Idle,
        Searching,
        MovingToTarget,
        Repairing,
        Confused
    }

    [Header("Debug")]
    public State currentState = State.Idle;

    [Header("Environment")]
    public EnvironmentControl2D environment;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectRange = 8f;
    public float arriveDistance = 0.5f;
    public float wanderRadius = 3f;

    [Header("Repair")]
    public float repairTime = 2.5f;

    private Rigidbody2D rb;
    private Transform target;
    private float repairTimer;
    private Vector2 wanderTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        ChooseNewWanderTarget();
    }

    void Update()
    {
        // Light-based state: if dim, go Confused
        if (environment != null && environment.lightLevel < 0.4f)
            currentState = State.Confused;

        switch (currentState)
        {
            case State.Idle:
                LookForTarget();
                break;

            case State.Searching:
            case State.MovingToTarget:
                MoveToTarget();
                break;

            case State.Repairing:
                DoRepair();
                break;

            case State.Confused:
                Wander();
                break;
        }
    }

    void LookForTarget()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("RepairTarget");
        if (candidates.Length == 0) return;

        GameObject nearest = null;
        float bestDist = Mathf.Infinity;

        foreach (GameObject go in candidates)
        {
            RepairTarget2D rt = go.GetComponent<RepairTarget2D>();
            if (rt != null && !rt.isDamaged)
                continue; // ignore fixed machines

            float d = Vector2.Distance(transform.position, go.transform.position);
            if (d < bestDist && d <= detectRange)
            {
                bestDist = d;
                nearest = go;
            }
        }

        if (nearest != null)
        {
            target = nearest.transform;
            currentState = State.MovingToTarget;
        }
    }

    void MoveToTarget()
    {
        if (target == null)
        {
            currentState = State.Idle;
            return;
        }

        Vector2 pos = rb.position;
        Vector2 dest = target.position;
        Vector2 dir = (dest - pos).normalized;

        rb.MovePosition(pos + dir * moveSpeed * Time.deltaTime);

        float dist = Vector2.Distance(pos, dest);
        if (dist <= arriveDistance)
        {
            currentState = State.Repairing;
            repairTimer = 0f;
        }
    }

    void DoRepair()
    {
        if (target == null)
        {
            currentState = State.Idle;
            return;
        }

        RepairTarget2D rt = target.GetComponent<RepairTarget2D>();
        if (rt != null)
        {
            rt.RepairTick(40f * Time.deltaTime);

            if (!rt.isDamaged)
            {
                currentState = State.Idle;
                target = null;
                return;
            }
        }
        else
        {
            // Fallback: timer-based repair
            repairTimer += Time.deltaTime;
            if (repairTimer >= repairTime)
            {
                Debug.Log(name + " finished repairing " + target.name);
                currentState = State.Idle;
                target = null;
            }
        }
    }

    void Wander()
    {
        Vector2 pos = rb.position;
        Vector2 dir = (wanderTarget - pos).normalized;
        rb.MovePosition(pos + dir * moveSpeed * 0.7f * Time.deltaTime);

        if (Vector2.Distance(pos, wanderTarget) < 0.5f)
            ChooseNewWanderTarget();

        if (environment != null && environment.lightLevel >= 0.4f)
        {
            currentState = State.Idle;
            target = null;
        }
    }

    void ChooseNewWanderTarget()
    {
        Vector2 center = transform.position;
        wanderTarget = center + Random.insideUnitCircle * wanderRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}