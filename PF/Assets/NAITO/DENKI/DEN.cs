using UnityEngine;
using System.Collections;

public class DEN : MonoBehaviour
{
    public float stunDuration = 3f;
    public GameObject stunEffectPrefab;
    public float effectHeightOffset = 1.0f; // エフェクトを生成する高さのオフセット

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                StartCoroutine(StunPlayer(playerController));
            }
        }
    }

    private IEnumerator StunPlayer(PlayerController player)
    {
        Vector3 effectPosition = player.transform.position + new Vector3(0, effectHeightOffset, 0);
        GameObject effectInstance = null;
        if (stunEffectPrefab != null)
        {
            effectInstance = Instantiate(stunEffectPrefab, effectPosition, Quaternion.identity, player.transform);
        }

        player.enabled = false;

        yield return new WaitForSeconds(stunDuration);

        player.enabled = true;

        if (effectInstance != null)
        {
            Destroy(effectInstance);
        }
    }
}
