#nullable enable
using System;
using Connector;
using Connector.Messages;
using Debug = UnityEngine.Debug;

namespace Cynteract.InputDevices
{
    public class CynteractDevice
    {
        private Device device;
        public string Id => device.Id;
        public Information? Information { get; private set; }
        public DeviceType DeviceType => Information?.deviceType switch
        {
            "leftGlove" => DeviceType.GloveLeft,
            "rightGlove" => DeviceType.GloveRight,
            "strap" => DeviceType.Strap,
            "cushion" => DeviceType.Cushion,
            _ => DeviceType.Unknown,
        };
        public Dataframe? LastData { get; private set; }
        public bool IsReady { get; private set; }
        public bool ReceivedInformationFromDevice { get; private set; }
        public DateTime LastInformationRequest { get; set; }

        public event Action<Dataframe>? OnData;
        public event Action<Information>? OnInformation;
        public event Action? OnReady;

        private DeviceInformationCache deviceInformationCache = new();


        public CynteractDevice(Device device)
        {
            this.device = device;
            Information = deviceInformationCache.Load(device.Id);
            if (Information != null)
                OnInformation?.Invoke(Information);
            device.OnMessage += OnMessage;
            device.OnError += error => Debug.LogError(error);
        }

        public void OnMessage(object message)
        {
            // Debug.Log("Message Received");
            switch (message)
            {
                case Connector.Messages.Information info:
                    Debug.Log("Device received information");
                    Information = info;
                    ReceivedInformationFromDevice = true;
                    deviceInformationCache.Save(Id, info);
                    OnInformation?.Invoke(info);
                    break;
                case Connector.Messages.Dataframe data:
                    LastData = data;

                    OnData?.Invoke(data);

                    if (!IsReady && Information != null)
                    {
                        OnReady?.Invoke();
                        IsReady = true;
                    }
                    if (!ReceivedInformationFromDevice && DateTime.Now > LastInformationRequest.AddSeconds(1))
                    {
                        Debug.Log("Data arrived, requesting Information");
                        device.SendMessage(new InformationRequest());
                        LastInformationRequest = DateTime.Now;
                    }
                    break;
            }
        }
    }
}
