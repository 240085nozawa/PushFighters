using UnityEngine;

public class BulletCntroller : MonoBehaviour
{
    public float lifeTime = 3f;

   
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        
        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     // ìGÇ∆é©ï™é©êgÇè¡Ç∑
        //     Destroy(collision.gameObject);
        //     Destroy(gameObject);
        // }

       Destroy(gameObject);
    }
}
