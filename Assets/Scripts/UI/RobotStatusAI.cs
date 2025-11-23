using UnityEngine;
using TMPro;

public class RobotStatusUI : MonoBehaviour
{
    public RobotAI2D[] robots;
    public TMP_Text statusText;

    void Update()
    {
        if (!statusText || robots == null) return;

        string s = "";
        foreach (RobotAI2D r in robots)
        {
            s += r.name + ": " + r.currentState.ToString() + "\n";
        }

        statusText.text = s;
    }
}