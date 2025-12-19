using Connector.Messages;
using UnityEngine;
public static class QuaternionFunctions
{
    public static Quaternion IMUDataToQuaternion(Dataframe.IMUData data)
    {
        return new Quaternion(data.x, data.y, data.z, data.w);
    }
    public static (Quaternion twist, Quaternion swing) SwingTwist(Quaternion rotation, Vector3 axis)
    {
        var twist = Twist(rotation, axis);
        var swing = rotation * Conjugate(twist);
        return (twist, swing);
    }
    public static Quaternion Twist(Quaternion rotation, Vector3 axis)
    {
        Vector3 rotationAxis = new Vector3(rotation.x, rotation.y, rotation.z);
        Vector3 projectedVector = Vector3.Project(rotationAxis, axis);
        var twist = new Quaternion(projectedVector.x, projectedVector.y, projectedVector.z, rotation.w);
        twist.Normalize();
        return twist;
    }
    public static float GetSignedAngleArroundAxis(Quaternion rotation, Vector3 axis, Vector3 rotatedVector)
    {
        var twist = Twist(rotation, axis);
        return Vector3.SignedAngle(rotatedVector, twist * rotatedVector, axis);
    }
    public static float GetSignedAngleArroundAxis(Quaternion rotation, Vector3 axis, Vector3 rotatedVector, float threshold)
    {
        var signedAngle = GetSignedAngleArroundAxis(rotation, axis, rotatedVector);
        return ToSignedAngle(ToUnsignedAngle(signedAngle), threshold);
    }

    /// <summary>
    /// Takes an angle between 0 and 360� and turns it into an angle between -180� and 180�
    /// </summary>
    /// <param name="angle">The angle (between 0 and 360�)</param>
    /// <returns>An angle between -180� and 180�</returns>
    public static float ToSignedAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }
    /// <summary>
    /// Takes an angle between 0 and 360� and turns it into an angle between <c>360-threshold</c> and <c>threshold</c>
    /// </summary>
    /// <param name="angle">The angle (between 0 and 360�)</param>
    /// <param name="threashold">The threshold where the sign should change</param>
    /// <returns>An angle between <c>360-threshold</c> and <c>threshold</c></returns>
    public static float ToSignedAngle(float angle, float threashold)
    {
        if (angle > threashold)
        {
            angle -= 360;
        }
        return angle;
    }
    /// <summary>
    /// Takes an angle between -180� and 180� and turns it into an angle between 0 and 360�
    /// </summary>
    /// <param name="signedAngle">The signed angle (between -180� and 180�)</param>
    /// <returns>An angle between 0 and 360�</returns>
    public static float ToUnsignedAngle(float signedAngle)
    {
        if (signedAngle < 0)
        {
            signedAngle += 360;
        }
        return signedAngle;
    }
    public static Quaternion Conjugate(Quaternion rotation)
    {
        var rot = new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        return rot.normalized;
    }
    public static Quaternion GetRelativeRotation(Quaternion rotation, Quaternion baseRotation)
    {
        return Quaternion.Inverse(baseRotation) * rotation;
    }
    public static bool IsEqual(Quaternion left, Quaternion right)
    {
        return left.x == right.x && left.y == right.y && left.z == right.z && left.w == right.w;
    }
}
