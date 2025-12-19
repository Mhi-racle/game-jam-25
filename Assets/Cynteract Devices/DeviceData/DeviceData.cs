using Connector;
using Cynteract.InputDevices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Connector.Messages;

public class DeviceData : IDeviceData
{
    public const int MaximumFlipAngle = 60;


    public CynteractDevice Device { get; private set; }

    public DeviceData(CynteractDevice device)
    {
        Device = device;
    }

    public Dictionary<string, Quaternion>? Resets { get; private set; }
    public bool IsWristXFlipped { get; private set; }
    public bool IsWristYFlipped { get; private set; }
    public bool IsWristZFlipped { get; private set; }

    public void Reset()
    {
        var data = Device.LastData;
        if (data == null) return;

        var information = Device.Information;

        Resets = GetResetsAsDictionary(information, data);


        var rotatedRightVector = GetAbsoluteRotationOfPartOrDefault(FingerPart.palmCenter) * Vector3.right;
        float angleToVector3Left = Vector3.Angle(rotatedRightVector, Vector3.left);
        IsWristXFlipped = angleToVector3Left < MaximumFlipAngle;



        var rotatedUpVector = GetAbsoluteRotationOfPartOrDefault(FingerPart.palmCenter) * Vector3.up;
        float angleToVector3Down = Vector3.Angle(rotatedUpVector, Vector3.down);
        IsWristYFlipped = angleToVector3Down < MaximumFlipAngle;


        var rotatedForwardVector = GetAbsoluteRotationOfPartOrDefault(FingerPart.palmCenter) * Vector3.forward;
        float angleToVector3Back = Vector3.Angle(rotatedForwardVector, Vector3.back);
        IsWristZFlipped = angleToVector3Back < MaximumFlipAngle;


    }
    public Dictionary<string, Quaternion> GetResetsAsDictionary(Information information, Dataframe data)
    {
        if (information.imuPositions is null)
        {
            return new();
        }
        var dictionary = new Dictionary<string, Quaternion>();
        foreach (var item in information.imuPositions)
        {
            if (item != "")
            {
                dictionary.Add(item, Quaternion.Inverse(GetAbsoluteRotationOfPart(item, data)));
            }
        }
        return dictionary;
    }
    /// <summary>
    /// Returns the reset Rotation of a finger part relative to the reset Rotation of a base part or <c>Quaternion.identity</c> if there is not data.
    /// </summary>
    /// <param name="part">The finger part</param>
    /// <param name="basePart">The base part</param>
    /// <param name="data">The data to be used</param>
    public Quaternion GetResetRelativeRotationOfPartOrDefault(FingerPart part, FingerPart basePart)
    {
        return GetResetRelativeRotationOfPart(part, basePart) ?? Quaternion.identity;
    }
    /// <summary>
    /// Returns the reset Rotation of a finger part relative to the reset Rotation of a base part or <c>null</c> if there is not data.
    /// </summary>
    /// <param name="part">The finger part</param>
    /// <param name="basePart">The base part</param>
    /// <param name="data">The data to be used</param>
    public Quaternion? GetResetRelativeRotationOfPart(FingerPart part, FingerPart basePart)
    {
        var data = Device.LastData;
        if (data == null) return null;
        return GetResetRelativeRotationOfPart(part, basePart, data);
    }
    /// <summary>
    /// Returns the reset Rotation of a finger part relative to the reset Rotation of a base part
    /// </summary>
    /// <param name="part">The finger part</param>
    /// <param name="basePart">The base part</param>
    /// <param name="data">The data to be used</param>
    public Quaternion GetResetRelativeRotationOfPart(FingerPart part, FingerPart basePart, Dataframe data)
    {
        return Quaternion.Inverse(GetResetRotationOfPart(basePart, data)) * GetResetRotationOfPart(part, data);
    }
    /// <summary>
    /// Returns the reset Rotation of a finger part or <c>Quaternion.identity</c> if there is no data.
    /// </summary>
    /// <param name="fingerPart">The fingerpart</param>
    public Quaternion GetResetRotationOfPartOrDefault(FingerPart fingerPart)
    {
        return GetResetRotationOfPart(fingerPart) ?? Quaternion.identity;
    }
    /// <summary>
    /// Returns the reset Rotation of a finger part or <c>null</c> if there is no data.
    /// </summary>
    /// <param name="fingerPart">The fingerpart</param>
    public Quaternion? GetResetRotationOfPart(FingerPart fingerPart)
    {
        var data = Device.LastData;
        if (data == null) return null;
        return GetResetRotationOfPart(Enum.GetName(typeof(FingerPart), fingerPart), data);
    }
    /// <summary>
    /// Returns the reset Rotation of a finger part
    /// </summary>
    /// <param name="fingerPart">The fingerpart</param>
    /// <param name="data">The data to be used</param>
    public Quaternion GetResetRotationOfPart(FingerPart fingerPart, Dataframe data)
    {

        return GetResetRotationOfPart(Enum.GetName(typeof(FingerPart), fingerPart), data);
    }
    /// <summary>
    /// Returns the reset Rotation of a finger part
    /// </summary>
    /// <param name="partKey">The name of the fingerpart</param>
    /// <param name="data">The data to be used</param>
    public Quaternion GetResetRotationOfPart(string partKey, Dataframe data)
    {
        if (Resets is not null && Resets.ContainsKey(partKey))
        {
            var resetRotation = Resets[partKey] * GetAbsoluteRotationOfPart(partKey, data);
            //Unflip the Wrist if needed
            if (partKey == nameof(FingerPart.palmCenter))
            {
                var flipX = IsWristXFlipped ? -1 : 1;
                var flipY = IsWristYFlipped ? -1 : 1;
                var flipZ = IsWristZFlipped ? -1 : 1;
                return new Quaternion(
                    flipX * resetRotation.x,
                    flipY * resetRotation.y,
                    flipZ * resetRotation.z,
                    resetRotation.w);
            }
            return resetRotation;
        }
        return GetAbsoluteRotationOfPart(partKey, data);
    }

    /// <summary>
    /// Returns the relative Rotation of a finger part, relative to a base part or <c>Quaternion.identity</c> if there is no data
    /// </summary>
    /// <param name="part">The finger part</param>
    /// <param name="basePart">The base part</param>
    public Quaternion GetRelativeRotationOfPartOrDefault(FingerPart part, FingerPart basePart)
    {
        return GetRelativeRotationOfPart(part, basePart) ?? Quaternion.identity;
    }
    /// <summary>
    /// Returns the relative Rotation of a finger part, relative to a base part or <c>null</c> if there is no data
    /// </summary>
    /// <param name="part">The finger part</param>
    /// <param name="basePart">The base part</param>
    public Quaternion? GetRelativeRotationOfPart(FingerPart part, FingerPart basePart)
    {
        var data = Device.LastData;
        if (data == null) return null;
        return GetRelativeRotationOfPart(part, basePart, data);
    }
    /// <summary>
    /// Returns the relative Rotation of a finger part, relative to a base part
    /// </summary>
    /// <param name="part">The finger part</param>
    /// <param name="basePart">The base part</param>
    /// <param name="data">The data to be used</param>
    public Quaternion GetRelativeRotationOfPart(FingerPart part, FingerPart basePart, Dataframe data)
    {
        return Quaternion.Inverse(GetAbsoluteRotationOfPart(basePart, data)) * GetAbsoluteRotationOfPart(part, data);
    }

    /// <summary>
    /// Returns the absolute rotation of a fingerpart in Unity coordinates or <c>Quaternion.identity</c> if there is no data
    /// </summary>
    /// <param name="fingerPart">The fingerpart</param>
    public Quaternion GetAbsoluteRotationOfPartOrDefault(FingerPart fingerPart)
    {
        return GetAbsoluteRotationOfPart(fingerPart) ?? Quaternion.identity;
    }
    /// <summary>
    /// Returns the absolute rotation of a fingerpart in Unity coordinates or <c>null</c> if there is no data
    /// </summary>
    /// <param name="fingerPart">The fingerpart</param>
    public Quaternion? GetAbsoluteRotationOfPart(FingerPart fingerPart)
    {
        var data = Device.LastData;
        if (data == null) return null;
        return GetAbsoluteRotationOfPart(fingerPart, data);
    }
    /// <summary>
    /// Returns the absolute rotation of a fingerpart in Unity coordinates.
    /// </summary>
    /// <param name="fingerPart">The fingerpart</param>
    /// <param name="data">The data to be used</param>
    public Quaternion GetAbsoluteRotationOfPart(FingerPart fingerPart, Dataframe data)
    {
        return GetAbsoluteRotationOfPart(Enum.GetName(typeof(FingerPart), fingerPart), data);
    }
    /// <summary>
    /// Returns the absolute rotation of a fingerpart in Unity coordinates.
    /// </summary>
    /// <param name="partKey">The name of the fingerpart</param>
    /// <param name="data">The data to be used</param>
    public Quaternion GetAbsoluteRotationOfPart(string partKey, Dataframe data)
    {
        var information = Device.Information;
        var mappings = information.imuPositions;
        if (mappings is null) return Quaternion.identity;
        var index = Array.IndexOf(mappings, partKey);
        if (index == -1) return Quaternion.identity;
#warning add this information in the firmware
        if (mappings.Contains("indexCenter"))
        {
            //V2 Glove
            return ToUnityCoordinatesV2(QuaternionFunctions.IMUDataToQuaternion(data.imuValues[index]));
        }

        //V3 glove
        if (partKey == "palmCenter")
        {
            return ToUnityCoordinatesV3Palm(QuaternionFunctions.IMUDataToQuaternion(data.imuValues[index]));
        }
        return ToUnityCoordinatesV3Finger(QuaternionFunctions.IMUDataToQuaternion(data.imuValues[index]));
    }
    #region Static Calculations

    /// <summary>
    /// Turns a Glove Rotation to a Quaternion in Unity Coordinatates.
    /// </summary>
    /// <returns></returns>
    public static Quaternion ToUnityCoordinatesV2(Quaternion raw)
    {
        return new Quaternion(raw.y, -raw.z, -raw.x, raw.w);

        //Old Mapping
        //return new Quaternion(raw.y, -raw.x, raw.z, -raw.w);
    }

    /// <summary>
    /// Turns a Glove Rotation to a Quaternion in Unity Coordinatates.
    /// </summary>
    /// <returns></returns>
    public static Quaternion ToUnityCoordinatesV3Finger(Quaternion raw)
    {
        return new Quaternion(raw.x, -raw.z, raw.y, raw.w);

        //Old Mapping
        //return new Quaternion(raw.y, -raw.x, raw.z, -raw.w);
    }
    /// <summary>
    /// Turns a Glove Rotation to a Quaternion in Unity Coordinatates.
    /// </summary>
    /// <returns></returns>
    public static Quaternion ToUnityCoordinatesV3Palm(Quaternion raw)
    {
        var finger = ToUnityCoordinatesV3Finger(raw);
        return new Quaternion(-finger.x, finger.y, -finger.z, raw.w);
        //return new Quaternion(-raw.y, -raw.z, raw.x, raw.w) * Quaternion.Euler(0,90,180);

        //Old Mapping
        //return new Quaternion(raw.y, -raw.x, raw.z, -raw.w);
    }


    #endregion
}
