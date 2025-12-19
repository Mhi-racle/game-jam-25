using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EggMovement : MonoBehaviour
{
    public float moveForce = 20f;
    public float maxSpeed = 6f;
    public float deadZone = 0.05f;

    private Rigidbody rb;

    // DEBUG
    private Vector3 accel;
    private Vector3 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 3f;
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.gameStarted)
        {
            return;
        }
        accel = Input.acceleration;
        velocity = rb.linearVelocity;

        float x = accel.x;
        float z = accel.y;

        if (Mathf.Abs(x) < deadZone) x = 0f;
        if (Mathf.Abs(z) < deadZone) z = 0f;

        Vector3 force = new Vector3(x, 0f, z) * moveForce;
        rb.AddForce(force, ForceMode.Acceleration);

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 clamped = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 20), $"Accel RAW: {accel}");
        GUI.Label(new Rect(10, 30, 400, 20), $"Velocity: {velocity}");
    }
}
