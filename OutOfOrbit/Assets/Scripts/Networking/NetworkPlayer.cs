using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
public class NetworkPlayer : MonoBehaviour
{
    [SerializeField] private Transform _Head;
    [SerializeField] private Transform _RightHand;
    [SerializeField] private Animator _RightAnimator;
    [SerializeField] private Transform _LeftHand;
    [SerializeField] private Animator _LeftAnimator;
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
            if (XRPlayer.current)
            {
                MapPositionByTrackedXRDevice(_Head, XRPlayer.current.head);
                MapPositionByTrackedXRDevice(_RightHand, XRPlayer.current.rightHand);
                MapPositionByTrackedXRDevice(_LeftHand, XRPlayer.current.leftHand);

                transform.position = XRPlayer.current.transform.localPosition;
                transform.rotation = XRPlayer.current.transform.localRotation;
            }

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), _RightAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), _LeftAnimator);
        }
    }

    private void MapPositionByTrackedXRDevice(Transform target, TrackedXRDevice device)
    {
        if (device == null)
            return;

        target.transform.localPosition = device.position;
        target.transform.localRotation = device.rotation;
    }

    private void UpdateHandAnimation(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            handAnimator.SetFloat("Trigger", triggerValue);
        else
            handAnimator.SetFloat("Trigger", 0);
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            handAnimator.SetFloat("Grip", gripValue);
        else
            handAnimator.SetFloat("Grip", 0);
    }
}