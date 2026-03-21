using JetBrains.Annotations;
using UnityEngine;

public class NoFlyZoneArea : MonoBehaviour
{
    GameManager gameManager;
    public bool isPlayerInside;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        isPlayerInside = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            gameManager.NoFlyZoneTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
        gameManager.OnFlyZoneExit();
    }
}
