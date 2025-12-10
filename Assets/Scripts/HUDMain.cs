using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HUDMain : MonoBehaviour
{
    [SerializeField] private RectTransform m_window;
    [SerializeField] private Transform controller;

    void Update()
    {
        Quaternion rotation_offset = Quaternion.Euler(45, 0, 0);
        m_window.position = controller.TransformPoint(new Vector3(0f, 0.1f, 0f));
        m_window.rotation = controller.rotation * rotation_offset;

        InputDevice mainGauche = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

    }
}
