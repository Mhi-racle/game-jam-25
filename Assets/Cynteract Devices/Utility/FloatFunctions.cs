using UnityEngine;

namespace Cynteract.InputDevices
{
    public static class FloatFunctions 
    {
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return Mathf.Clamp(RemapUnclamped(value, fromMin, fromMax, toMin, toMax), toMin, toMax);
        }
        public static float RemapUnclamped(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

    }
}


