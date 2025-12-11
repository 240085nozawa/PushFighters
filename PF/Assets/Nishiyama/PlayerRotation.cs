using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float rotationSpeed = 10f;
    private Vector3 currentRotationDirection = Vector3.forward;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

              if (inputDirection.magnitude >= 0.1f)
        {
                       currentRotationDirection = inputDirection;
        }

        RotatePlayer(currentRotationDirection);
    }

    
    void RotatePlayer(Vector3 direction)
    {
         Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp
            (
            transform.rotation,       
            targetRotation,             
            Time.deltaTime * rotationSpeed 
            );
    }

}
