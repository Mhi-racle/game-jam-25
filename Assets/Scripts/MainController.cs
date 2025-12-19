using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] private Transform YRotate, xzRotate;
    [SerializeField] private float ySpeed, xzSpeed;
    [SerializeField] private float tiltAngle;

    private void Update()
    {
        float t = Time.time * xzSpeed;

        float xRot = Mathf.Sin(t) * tiltAngle;
        float zRot = Mathf.Cos(t) * tiltAngle;

        YRotate.Rotate(0f, ySpeed * Time.deltaTime, 0f, Space.World);
        xzRotate.localRotation = Quaternion.Euler(xRot, 0f, zRot);
        
    }
}
