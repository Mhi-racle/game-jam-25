using Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Connector.Messages;
#nullable enable
namespace Cynteract.InputDevices
{
    public class CynteractDeviceManager : MonoBehaviour
    {
        DeviceManager deviceManager = new();

        public static CynteractDeviceManager Instance { get; private set; }


        private ThreadSafeDataNotifier<CynteractDevice> onDeviceConnectedNotifier = new();
        private ThreadSafeDataNotifier<CynteractDevice> onDeviceDisconnectedNotifier = new();
        private ThreadSafeDataNotifier<CynteractDevice> onDeviceReadyNotifier = new();
        private ThreadSafeDataNotifier<CynteractDevice, Information> onDeviceInformationNotifier = new();
        public Dictionary<string, CynteractDevice> Devices { get; private set; } = new();

        private void Awake()
        {
            if (Instance is not null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }

        void Start()
        {

            deviceManager.OnDeviceConnected += device =>
            {
                var cynteractDevice = new CynteractDevice(device);
                Devices[cynteractDevice.Id] = cynteractDevice;
                onDeviceConnectedNotifier.NotifyListeners(cynteractDevice);
                cynteractDevice.OnReady += () => onDeviceReadyNotifier.NotifyListeners(cynteractDevice);
                cynteractDevice.OnInformation += info => onDeviceInformationNotifier.NotifyListeners(cynteractDevice, info);
            };
            deviceManager.OnDeviceDisconnected += device =>
            {
                if (Devices.ContainsKey(device.Id))
                {
                    onDeviceDisconnectedNotifier.NotifyListeners(Devices[device.Id]);
                    Devices.Remove(device.Id);
                }
            };
            deviceManager.OnError += e => Debug.LogError(e);

            deviceManager.Start();
        }

        public void StartDeviceManager()
        {
            deviceManager.Start();
        }
        public void StopDeviceManager()
        {
            deviceManager.Stop();
        }

        public EventSubscription ListenOnConnected(Action<CynteractDevice> onConnected)
        {
            var subscription = onDeviceConnectedNotifier.Listen(onConnected);
            foreach (var device in Devices.Values)
            {
                onConnected(device);
            }
            return subscription;
        }
        public EventSubscription ListenOnReady(Action<CynteractDevice> onReady)
        {
            var subscription = onDeviceReadyNotifier.Listen(onReady);
            foreach (var device in Devices.Values)
            {
                if (device.IsReady)
                {
                    onReady(device);
                }
            }
            return subscription;
        }
        public EventSubscription ListenOnInformation(Action<CynteractDevice, Information> onInformation)
        {
            var subscription = onDeviceInformationNotifier.Listen(onInformation);
            foreach (var device in Devices.Values)
            {
                if (device.Information != null)
                {
                    onInformation(device, device.Information);
                }
            }
            return subscription;
        }
        public EventSubscription ListenOnDisconnected(Action<CynteractDevice> onDisconnected)
        {
            var subscription = onDeviceDisconnectedNotifier.Listen(onDisconnected);
            return subscription;
        }
        private void OnDestroy()
        {
            if (Instance == this)
            {
                deviceManager.Stop();
            }
        }


    }
}
