using Cynteract.InputDevices;
using UnityEngine;
/// <summary>
/// Simple script to rotate a cube with cushion rotation
/// </summary>
public class SimpleCushionDisplayer : MonoBehaviour
{
    [SerializeField]
    private Transform referenceCube;
    private CushionData cushionData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Make sure a CynteractDeviceManager script is present in the scene
        //Wait for the device to be ready
        CynteractDeviceManager.Instance.ListenOnReady(device =>
        {
            //Get the corresponding data
            //This assumes, the device is a cushion
            cushionData = new CushionData(device);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (cushionData == null)
        {
            return;
        }
        // Get the absolute (unmodified) rotation of the cushions main and only sensor (Palm Center bc it was on a glove first)
        var rotation = cushionData.GetAbsoluteRotationOfPartOrDefault(FingerPart.palmCenter);
        //Set the rotation of the cube to the rotation of the cushion
        referenceCube.rotation = rotation;
    }
}
