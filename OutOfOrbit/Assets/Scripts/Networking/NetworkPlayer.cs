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
    private XROrigin _myOrigin;
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
            _myOrigin = FindObjectOfType<XROrigin>();
            if (_myOrigin == null) Debug.LogWarning($"XROrigin could not be found in scene.");
        }
        else
            name = $"NetworkPlayer (Client {_PhotonView.CreatorActorNr})";
    }
    private void Update()
    {
        if (_PhotonView.IsMine)
        {
            if (Settings.playMode == PlayMode.CAVE)
                MapPositionByGameObject(_Head, PlayerHandler.Head);
            else
                MapPosition(_Head, XRNode.Head);

            MapPosition(_RightHand, XRNode.RightHand);
            MapPosition(_LeftHand, XRNode.LeftHand);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), _RightAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), _LeftAnimator);

            if (_myOrigin == null) return;
            transform.position = _myOrigin.transform.localPosition;
            transform.rotation = _myOrigin.transform.localRotation;
        }
    }

    private void MapPositionByGameObject(Transform target, GameObject device)   //< Only use for CAVE, you will get weird results when using with HMD, this was developed as a solution for just the CAVE
    {
        if (device == null)
            return;

        target.transform.localPosition = device.transform.position;
        target.transform.localRotation = device.transform.rotation;     //TODO: Needs to be rotated as the default ViveTracker rotation is forward and not up :(
    }

    private void MapPosition(Transform target, XRNode node)
    {
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
        target.transform.localPosition = position;
        target.transform.localRotation = rotation;
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