using Connector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable
namespace Cynteract.InputDevices
{
    public class CushionData : DeviceData
    {
        public CushionData(CynteractDevice device) : base(device)
        { }
        public float GetXAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            var wristRotation = GetResetRotationOfPart(FingerPart.palmCenter, data);
            return QuaternionFunctions.GetSignedAngleArroundAxis(wristRotation, -Vector3.right, Vector3.forward);
        }
        public float GetYAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            var wristRotation = GetResetRotationOfPart(FingerPart.palmCenter, data);
            return QuaternionFunctions.GetSignedAngleArroundAxis(wristRotation, Vector3.up, Vector3.forward);
        }
        public float GetZAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            var wristRotation = GetResetRotationOfPart(FingerPart.palmCenter, data);
            return QuaternionFunctions.GetSignedAngleArroundAxis(wristRotation, -Vector3.forward, Vector3.up);
        }


        public float SumAll()
        {
            return GetXAngle() + GetYAngle() + GetZAngle();
        }
    }
}
