// shamelessly copied Gabler's custom controller script from the third sem workshop
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class CustomMoveController : MonoBehaviour
{
    public XRNode inputSourceFromDevice;
    private float fSpeed = 1.0f;
    private Vector2 input2DAxis; 
    private CharacterController HMDPlayer;
    private XROrigin myOrigin;

    // Start is called before the first frame update
    void Start()
    {
        HMDPlayer = GetComponent<CharacterController>();
        myOrigin = GetComponent<XROrigin>();
    }

    
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    private void FixedUpdate()
    {
        //too tired to think of how to get that ref to work
        //Quaternion hmdYaw = Quaternion.Euler(0, myOrigin.cameraGameObject.transform.eulerAngles.y, 0);
        //Vector3 direction = hmdYaw * new Vector3(input2DAxis.x, 0, input2DAxis.y);
        //HMDPlayer.Move(direction * Time.fixedDeltaTime * fSpeed);
    }
    
    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSourceFromDevice);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out input2DAxis);
    }
}
