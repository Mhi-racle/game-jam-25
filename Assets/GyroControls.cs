using UnityEngine;

public class GyroControls : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = GyroToUnity(Input.gyro.attitude);
        //Uncomment for accelerometer
        //print(Input.acceleration);
        //GetComponent<Rigidbody>().AddForce(Input.acceleration);
    }
    /// <summary>
    /// Convert coordinate system of gyro to that of unity. Based on unity documentation
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
