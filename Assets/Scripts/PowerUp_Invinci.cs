using UnityEngine;

public class InvincibilityPowerUp : MonoBehaviour
{
    public float iFrames = 4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();

            if (player != null)
            {
                player.ActivateInvincibility(iFrames);
                SoundManager.Play("PowerUp");

                GameAnalyticsManager.instance.FunnelFinished(3, "or 3 Player_Obtained_Powerup");
            }

            Destroy(gameObject);
        }
    }
}
