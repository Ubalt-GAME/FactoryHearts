using UnityEngine;

public class RepairTarget2D : MonoBehaviour
{
    [Header("Repair State")]
    public bool isDamaged = true;        // robots should work on damaged machines
    public float repairProgress = 0f;
    public float repairGoal = 100f;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    // Called by robots every frame while repairing
    public void RepairTick(float amount)
    {
        if (!isDamaged)
            return;

        repairProgress += amount;

        if (repairProgress >= repairGoal)
        {
            repairProgress = repairGoal;
            isDamaged = false;
            UpdateColor();
            Debug.Log(name + " is now FIXED!");

            // :small_blue_diamond: Tell the game manager this machine was repaired
            if (FactoryGameManager.Instance != null)
            {
                FactoryGameManager.Instance.RegisterRepair(this);
            }
        }
    }

    // Called by drones when they attack a fixed machine
    public void Damage()
    {
        // Only do something if it was actually fixed
        if (!isDamaged)
        {
            isDamaged = true;
            repairProgress = 0f;
            UpdateColor();
            Debug.Log(name + " was DAMAGED by a drone!");
        }
    }

    void UpdateColor()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (sr == null) return;

        // Yellow = damaged, Green = fixed
        sr.color = isDamaged
            ? new Color(1f, 0.85f, 0.3f)   // damaged
            : new Color(0.4f, 1f, 0.4f);   // fixed
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        sr = GetComponent<SpriteRenderer>();
        UpdateColor();
    }
#endif
}