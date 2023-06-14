using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
public class NetworkPlayer : MonoBehaviour
{
    [SerializeField] private Transform _Head;
    [SerializeField] private Transform _RightHand;
    [SerializeField] private Transform _LeftHand;
    private XROrigin _myOrigin;
    private PhotonView _PhotonView;
    private void Start()
    {
        _PhotonView = GetComponent<PhotonView>();
        if (_PhotonView.IsMine)
        {
            foreach (var renderer in GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
            _myOrigin = FindObjectOfType<XROrigin>();
            if (_myOrigin == null) Debug.LogWarning($"XROrigin could not be found in scene.");
        }
    }
    private void Update()
    {
        if (_PhotonView.IsMine)
        {
            MapPosition(_Head, XRNode.Head);
            MapPosition(_RightHand, XRNode.RightHand);
            MapPosition(_LeftHand, XRNode.LeftHand);

            if (_myOrigin == null) return;
            transform.position = _myOrigin.transform.position;
            transform.rotation = _myOrigin.transform.rotation;
        }
    }
    private void MapPosition(Transform target, XRNode node)
    {
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
        target.transform.localPosition = position;
        target.transform.localRotation = rotation;
    }
}