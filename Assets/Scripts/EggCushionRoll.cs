using Cynteract.InputDevices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EggCushionRoll : MonoBehaviour
{
    [Header("Movement")]
    public float moveForce = 15f;
    public float maxSpeed = 6f;
    public float deadZone = 0.05f;

    [Header("Physics")]
    public float drag = 3f;

    private Rigidbody rb;
    private CushionData cushionData;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = drag;

        // Wait for cushion device
        CynteractDeviceManager.Instance.ListenOnReady(device =>
        {
            cushionData = new CushionData(device);
        });
    }

    private void FixedUpdate()
    {
        if (cushionData == null)
            return;

        // Get cushion rotation
        Quaternion rotation =
            cushionData.GetAbsoluteRotationOfPartOrDefault(FingerPart.palmCenter);

        // Convert rotation to tilt (Unity space)
        Vector3 tilt = rotation * Vector3.forward;

        float x = tilt.x;
        float z = tilt.z;

        // Deadzone to avoid drift
        if (Mathf.Abs(x) < deadZone) x = 0f;
        if (Mathf.Abs(z) < deadZone) z = 0f;

        // Apply rolling force
        Vector3 force = new Vector3(x, 0f, z) * moveForce;
        rb.AddForce(force, ForceMode.Acceleration);

        // Clamp speed
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 clamped = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
    }
}
