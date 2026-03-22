using UnityEngine;

public class FunnelEvent : MonoBehaviour
{
    [SerializeField] int stepNumber;
    [SerializeField] string stepName;

    public void TrackFinishFunnel()
    {
        GameAnalyticsManager.instance.FunnelFinished(stepNumber, stepName);
    }
}
