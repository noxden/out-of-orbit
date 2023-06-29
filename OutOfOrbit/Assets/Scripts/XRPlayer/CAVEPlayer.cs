using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAVEPlayer : XRPlayer
{
    public override PlayerType type { get => PlayerType.CAVE; }
    private Vector3 viveTrackerRotationCorrectionVector = new Vector3(90, 0, 0);
    private Transform helperTransform;

    protected override void Start()
    {
        base.Start();
        helperTransform = new GameObject().transform;
        helperTransform.SetParent(this.transform, worldPositionStays: false);
        helperTransform.name = "NetworkPlayer Head Rotation Helper";
    }

    public override Pose GetDevicePose(XRDeviceType deviceType)
    {
        switch (deviceType)
        {
            case XRDeviceType.Head:
                {
                    bool isTracked = !(head.pose.position == Vector3.zero && head.pose.rotation == Quaternion.identity);
                    // Debug.Log($"Head is {(isTracked ? "" : "not")} tracked.");

                    if (isTracked)
                    {
                        helperTransform.localPosition = head.transform.localPosition;
                        helperTransform.localRotation = head.transform.localRotation;
                        helperTransform.Rotate(viveTrackerRotationCorrectionVector, Space.Self);
                        return new Pose(helperTransform.localPosition, helperTransform.localRotation);
                    }
                    return head.pose;
                }
            case XRDeviceType.LeftHand: return leftHand.pose;
            case XRDeviceType.RightHand: return rightHand.pose;
            default: return Pose.identity;
        }
    }
}
