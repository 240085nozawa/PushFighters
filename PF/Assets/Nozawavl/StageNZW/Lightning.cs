using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public float stunTime = 1.0f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        Debug.Log("? Lightning hit! Player stunned");

        // ★ Lightning は自分でコルーチンを動かさない！
        // ★ Player がコルーチンを実行するので Pause されない
        pc.StartCoroutine(Recover(pc));

        pc.canMove = false;   // スタン開始

        Destroy(gameObject);
    }

    private IEnumerator Recover(PlayerController pc)
    {
        yield return new WaitForSeconds(stunTime);

        if (pc != null)
        {
            pc.canMove = true;   // スタン解除
            Debug.Log("? Lightning stun END");
        }
    }
}
