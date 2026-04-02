using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    public float speedMultiplier = 2f;
    public float duration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();

            if (player != null)
            {
                player.ActivateSpeedBoost(speedMultiplier, duration);
                SoundManager.Play("PowerUp");

                GameAnalyticsManager.instance.FunnelFinished(3, "or 3 Player_Obtained_Powerup");
            }

            Destroy(gameObject);
        }
    }
}