using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum XRDeviceType { Undefined, Head, LeftHand, RightHand }

public class TrackedXRDevice : MonoBehaviour
{
    [Tooltip("Which tracked device does this object represent?")]
    [SerializeField] private XRDeviceType _type;
    public XRDeviceType type { get => _type; }

    public Pose pose { get => TransformToPose(this.transform); }
    public Vector3 position { get => transform.localPosition; }
    public Quaternion rotation { get => transform.localRotation; }

    private void Start()
    {
        if (type == XRDeviceType.Undefined)
            Debug.LogWarning($"[TrackedXRDevice] [{this.name}] Device type is still UNDEFINED. Please set it to whichever XR device {this.gameObject.name} represents.", this.gameObject);
        
        if (!TryGetComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>(out _) && !TryGetComponent<UnityEngine.XR.Interaction.Toolkit.XRBaseController>(out _))
            Debug.LogWarning($"[TrackedXRDevice] [{this.name}] No tracking script could been found on {this.gameObject.name}. Are you sure that this script is on the correct GameObject?");
    }

    //# Private Methods 
    private static Pose TransformToPose(Transform transform)
    {
        return new Pose(transform.localPosition, transform.localRotation);
    }
}
