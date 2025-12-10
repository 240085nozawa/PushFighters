using UnityEngine;

public class StunHit : MonoBehaviour
{
    [Header("ƒXƒ^ƒ“Œp‘±ŽžŠÔ")]
    public float stunTime = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        // canMove ‚ð false ‚É‚µ‚Äˆê’èŽžŠÔŒã–ß‚·
        pc.canMove = false;
        pc.StartCoroutine(Recover(pc));

        Debug.Log($"StunHit: PlayerTag {pc.PlayerTag} stunned!");
    }

    private System.Collections.IEnumerator Recover(PlayerController pc)
    {
        yield return new WaitForSeconds(stunTime);
        if (pc != null) pc.canMove = true;
    }
}
