using Connector;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynteract.InputDevices;
#nullable enable
public class CynteractDeviceStatusDisplayer : MonoBehaviour
{
    [SerializeField]
    private Color gloveNotConnectedColor = Color.red;
    [SerializeField]
    private Color gloveConnectedColor = Color.cyan;
    [SerializeField]
    private Color gloveReadyColor = Color.green;

    [SerializeField]
    private Image? coloredDotImage;
    [SerializeField]
    private GameObject leftGloveIcon, rightGloveIcon, strapIcon, cushionIcon;
    private GameObject[] icons;
    ConcurrentQueue<Action> actionQueue = new();

    bool ready = false;
    bool connected = false;
    private EventSubscription onConnectedSubscription, onReadySubscription, onInformationSubscription, onDisconnectedSubscription;

    private void Awake()
    {
        icons = new GameObject[] { leftGloveIcon, rightGloveIcon, strapIcon, cushionIcon };
    }
    void Start()
    {
        SetColor(gloveNotConnectedColor);
        ShowIcon(leftGloveIcon);
        onConnectedSubscription = CynteractDeviceManager.Instance.ListenOnConnected(device =>
        {
            connected = true;
            UpdateColor();
            ShowIcon(leftGloveIcon);
        }
        );
        onReadySubscription = CynteractDeviceManager.Instance.ListenOnReady(device =>
            {
                ready = true;
                UpdateColor();
            });
        onInformationSubscription = CynteractDeviceManager.Instance.ListenOnInformation((device, _) =>
            {
                var icon = device.DeviceType switch
                {
                    Cynteract.InputDevices.DeviceType.Unknown => leftGloveIcon,
                    Cynteract.InputDevices.DeviceType.GloveLeft => leftGloveIcon,
                    Cynteract.InputDevices.DeviceType.GloveRight => rightGloveIcon,
                    Cynteract.InputDevices.DeviceType.Strap => strapIcon,
                    Cynteract.InputDevices.DeviceType.Cushion => cushionIcon,
                    _ => leftGloveIcon
                };
                ShowIcon(icon);

            });
        onDisconnectedSubscription = CynteractDeviceManager.Instance.ListenOnDisconnected(device =>
        {
            connected = false;
            ready = false;
            UpdateColor();
            ShowIcon(leftGloveIcon);

        });
    }

    void SetColor(Color color)
    {
        actionQueue.Enqueue(() =>
        {
            if (coloredDotImage != null)
            {
                coloredDotImage.color = color;
            }
        });

    }
    void UpdateColor()
    {
        if (ready)
        {
            SetColor(gloveReadyColor);
            return;
        }
        if (connected)
        {
            SetColor(gloveConnectedColor);
            return;
        }
        SetColor(gloveNotConnectedColor);

    }
    void ShowIcon(GameObject icon)
    {
        actionQueue.Enqueue(() =>
        {
            foreach (var item in icons)
            {
                if (icon == item)
                {
                    item.SetActive(true);
                }
                else
                {
                    item.SetActive(false);
                }
            }
        });
    }
    private void Update()
    {
        while (actionQueue.Count > 0)
        {
            if (actionQueue.TryDequeue(out Action action))
            {
                action();
            }
        }
    }
    private void OnDestroy()
    {
        onConnectedSubscription?.Remove();
        onReadySubscription?.Remove();
        onInformationSubscription?.Remove();
        onDisconnectedSubscription?.Remove();
    }
}
