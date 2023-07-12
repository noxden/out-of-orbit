using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
public class NetworkPlayer : MonoBehaviour
{
    [SerializeField] private Transform _Head;
    [SerializeField] private Transform _RightHand;
    [SerializeField] private PawAnimator _RightAnimator;
    [SerializeField] private Transform _LeftHand;
    [SerializeField] private PawAnimator _LeftAnimator;
    private PhotonView _PhotonView;
    private void Start()
    {
        _PhotonView = GetComponent<PhotonView>();
        if (_PhotonView.IsMine)
        {
            name = "NetworkPlayer (Mine)";
            if (!Settings.showOwnedNetworkPlayer)
            {
                foreach (var renderer in GetComponentsInChildren<Renderer>())
                    renderer.enabled = false;
            }
        }
        else
            name = $"NetworkPlayer (Client {_PhotonView.CreatorActorNr})";
    }
    private void Update()
    {
        if (_PhotonView.IsMine)
        {
            if (XRPlayer.current != null)
            {
                MapPositionByPose(_Head, XRPlayer.current.GetDevicePose(XRDeviceType.Head));
                MapPositionByPose(_RightHand, XRPlayer.current.GetDevicePose(XRDeviceType.RightHand));
                MapPositionByPose(_LeftHand, XRPlayer.current.GetDevicePose(XRDeviceType.LeftHand));

                transform.position = XRPlayer.current.transform.position;
                transform.rotation = XRPlayer.current.transform.rotation;
            }

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), _RightAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), _LeftAnimator);
        }
    }

    private void MapPositionByPose(Transform target, Pose pose)
    {
        target.transform.localPosition = pose.position;
        target.transform.localRotation = pose.rotation;
    }

    private void UpdateHandAnimation(InputDevice targetDevice, PawAnimator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            handAnimator.SetPawClasp(gripValue);
        else
            handAnimator.SetPawClasp(0);
    }
}