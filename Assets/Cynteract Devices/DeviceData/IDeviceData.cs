using Connector;
using Cynteract.InputDevices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeviceData
{
    public void Reset();
    public CynteractDevice Device { get; }
}