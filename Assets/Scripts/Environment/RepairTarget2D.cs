using UnityEngine;

public class RepairTarget2D : MonoBehaviour
{
    public bool isDamaged = true;
    public float repairProgress = 0f;
    public float repairGoal = 100f;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    public void RepairTick(float amount)
    {
        if (!isDamaged) return;

        repairProgress += amount;

        if (repairProgress >= repairGoal)
        {
            isDamaged = false;
            UpdateColor();
            Debug.Log(name + " is now FIXED!");
        }
    }

    void UpdateColor()
    {
        if (sr == null) return;

        // Yellow = damaged, Green = fixed
        sr.color = isDamaged
            ? new Color(1f, 0.85f, 0.3f)
            : new Color(0.4f, 1f, 0.4f);
    }
}