using Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Connector.Messages;
#nullable enable
namespace Cynteract.InputDevices
{
    public class GloveData : DeviceData
    {

        public GloveData(CynteractDevice device) : base(device)
        { }



        public float GetFistGrip()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            return
                 GetIndexBendAngle(data)
                + GetMiddleBendAngle(data)
                + GetRingBendAngle(data)
                + GetPinkyBendAngle(data);
        }
        public float GetIndexBendAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            return GetIndexBendAngle(data);
        }
        public float GetIndexBendAngle(Dataframe data)
        {

            return GetBendAngle(data, FingerPart.indexBase, FingerPart.palmCenter);
        }
        public float GetMiddleBendAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            return GetMiddleBendAngle(data);
        }
        public float GetMiddleBendAngle(Dataframe data)
        {
            return GetBendAngle(data, FingerPart.middleBase, FingerPart.palmCenter);

        }
        public float GetRingBendAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            return GetRingBendAngle(data);
        }
        public float GetRingBendAngle(Dataframe data)
        {
            return GetBendAngle(data, FingerPart.ringBase, FingerPart.palmCenter);

        }
        public float GetPinkyBendAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            return GetPinkyBendAngle(data);
        }
        private float GetPinkyBendAngle(Dataframe data)
        {
            return GetBendAngle(data, FingerPart.pinkyBase, FingerPart.palmCenter);
        }
        public float GetThumbBendAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            return GetThumbBendAngle(data);
        }
        public float GetThumbBendAngle(Dataframe data)
        {
            return GetBendAngle(data, FingerPart.thumbBase, FingerPart.palmCenter);
        }
        private float GetBendAngle(Dataframe data, FingerPart fingerPart, FingerPart basePart)
        {
            var relativeRotation = GetResetRelativeRotationOfPart(fingerPart, basePart, data);
            var bendAngle = QuaternionFunctions.GetSignedAngleArroundAxis(relativeRotation, Vector3.right, Vector3.forward, 270);
            return bendAngle;
        }
        public float GetWristYAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            var wristRotation = GetResetRotationOfPart(FingerPart.palmCenter, data);
            return QuaternionFunctions.GetSignedAngleArroundAxis(wristRotation, Vector3.up, Vector3.forward);
        }
        public float GetWristZAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            var wristRotation = GetResetRotationOfPart(FingerPart.palmCenter, data);
            return QuaternionFunctions.GetSignedAngleArroundAxis(wristRotation, -Vector3.forward, Vector3.up);
        }

        public float GetWristXAngle()
        {
            var data = Device.LastData;
            if (data == null) return 0.0f;
            var wristRotation = GetResetRotationOfPart(FingerPart.palmCenter, data);
            return QuaternionFunctions.GetSignedAngleArroundAxis(wristRotation, -Vector3.right, Vector3.forward);
        }
        public float SumAll()
        {
            return GetFistGrip() + GetWristZAngle() + GetWristYAngle() + GetWristXAngle();
        }
        public float GetIndexThumbPinch()
        {
            return GetIndexBendAngle() + GetThumbBendAngle();
        }

    }
}
