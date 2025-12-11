using UnityEngine;

public class TPSTracker : MonoBehaviour
{
    public Transform target;

    public float distance = 5.0f;

    public float height = 2.0f;

    public float followSpeed = 5.0f;

    public float lookSpeed = 7.0f;

    public int LookRotate = 0;
  
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("追尾するターゲットが設定されていません。");
            return;
        }

        Vector3 targetPosition = target.position + (Vector3.up * height);

        targetPosition.z += distance;

        transform.position = Vector3.Lerp(
            transform.position,    
            targetPosition,        
            followSpeed * Time.deltaTime 
        );

        transform.rotation = Quaternion.Euler(LookRotate, 0f, 0f);
     }
}
