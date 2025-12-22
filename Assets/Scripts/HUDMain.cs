using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HUDMain : MonoBehaviour
{
    [SerializeField] private RectTransform m_window;
    [SerializeField] private Transform controller;

    void Update()
    {

        InputDevice mainGauche = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (mainGauche.TryGetFeatureValue(CommonUsages.isTracked, out bool tracked) && tracked)
        {
            Quaternion rotation_offset = Quaternion.Euler(45, 0, 0);
            m_window.position = controller.TransformPoint(new Vector3(0f, 0.1f, 0f));
            m_window.rotation = controller.rotation * rotation_offset;
        }



        if (!mainGauche.isValid)
            return;

    }
}
