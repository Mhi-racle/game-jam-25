using Cynteract.InputDevices;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Script to rotate a cube with cushion rotation. Can be reset/zeroed
/// </summary>
public class ResetCushionDisplayer : MonoBehaviour
{
    [SerializeField]
    private Transform referenceCube;
    [SerializeField]
    Button resetButton;
    private CushionData cushionData;
    //Awake is called once before Start
    void Awake()
    {
        resetButton.onClick.AddListener(() =>
        {
            //Sets the ResetRotation to the current value
            cushionData?.Reset();
        });
    }
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
        // Get the reset rotation of the cushions main and only sensor (Palm Center bc it was on a glove first)
        // If no reset was done, this is the same as the absolute rotation. Otherwise its Quaternion.Inverse(ResetRotation)*AbsoluteRotation
        var rotation = cushionData.GetResetRotationOfPartOrDefault(FingerPart.palmCenter);
        //Set the rotation of the cube to the rotation of the cushion
        referenceCube.rotation = rotation;
    }
}
