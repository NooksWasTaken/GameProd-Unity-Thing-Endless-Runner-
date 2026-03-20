using UnityEngine;

public class InvincibilityPowerUp : MonoBehaviour
{
    public float iFrames = 4f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.ActivateInvincibility(iFrames);
            }

            Destroy(gameObject);
        }
    }
}
