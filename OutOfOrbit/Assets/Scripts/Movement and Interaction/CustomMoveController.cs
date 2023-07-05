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
    private const float fGravity = 9.8f; 
    public LayerMask groundLayer; 
 
    // Start is called before the first frame update 
    void Start() 
    { 
        HMDPlayer = GetComponent<CharacterController>(); 
        myOrigin = GetComponent<XROrigin>(); 
    } 
 
 
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled. 
    private void FixedUpdate() 
    { 
        Quaternion hmdYaw = Quaternion.Euler(0, myOrigin.Camera.transform.eulerAngles.y, 0); 
        Vector3 direction = hmdYaw * new Vector3(input2DAxis.x, 0, input2DAxis.y); 
        HMDPlayer.Move(direction * Time.fixedDeltaTime * fSpeed); 
 
        if (!IsGrounded()) 
        { 
            HMDPlayer.Move(Vector3.down * fGravity * Time.fixedDeltaTime); 
        } 
 
        CharacterFollowHeadset(); 
    } 
 
    // Update is called once per frame 
    void Update() 
    { 
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSourceFromDevice); 
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out input2DAxis); 
    } 
 
    void CharacterFollowHeadset() 
    { 
        HMDPlayer.height = myOrigin.CameraInOriginSpaceHeight + 0.2f; 
        Vector3 capsuleCenter = transform.InverseTransformPoint(myOrigin.Camera.transform.position); 
        HMDPlayer.center = new Vector3(capsuleCenter.x, HMDPlayer.height/2.0f + HMDPlayer.skinWidth, capsuleCenter.z); 
    } 
 
    private bool IsGrounded() 
    { 
        Vector3 centerPos = transform.TransformPoint(HMDPlayer.center); 
        float rayLength = HMDPlayer.center.y + 0.01f; 
        return Physics.SphereCast(centerPos, HMDPlayer.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer); 
    } 
}