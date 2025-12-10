using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public float lifetime = 0.5f; // š 0.5•b‚É•ÏX

    void Start()
    {
        // 0.5•bŒã‚É©g‚ğÁ–Å‚³‚¹‚é
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player2"))
        {
            return;
        }
    }
}
