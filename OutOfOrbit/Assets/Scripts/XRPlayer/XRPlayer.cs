using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { HMD, CAVE }

public abstract class XRPlayer : MonoBehaviour
{
    public static XRPlayer current { get; private set; } = null;
    public static XRPlayer[] all { get => FindObjectsByType<XRPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None); }
    public abstract PlayerType type { get; }
    public bool isActive { get { return gameObject.activeInHierarchy; } private set { gameObject.SetActive(value); } }

    public TrackedXRDevice head { get; protected set; }
    public TrackedXRDevice leftHand { get; protected set; }
    public TrackedXRDevice rightHand { get; protected set; }

    private void Start()
    {
        TrackedXRDevice[] trackedXRDevices = GetComponentsInChildren<TrackedXRDevice>();
        foreach (TrackedXRDevice device in trackedXRDevices)
        {
            switch (device.type)
            {
                case XRDeviceType.Head: head = device; break;
                case XRDeviceType.LeftHand: leftHand = device; break;
                case XRDeviceType.RightHand: rightHand = device; break;
                default: Debug.LogError($"XR device \"{device}\" is neither Head nor LeftHand or RightHand."); break;
            }
        }
        // Debug.Log($"[XRPlayer] [{this.name}] Identified {head} as head, {leftHand} as left hand and {rightHand} as right hand.");
    }

    //# Private Methods 
    /// <summary>
    /// Sets the XRPlayer which calls this method as the current player.
    /// </summary>
    private void MakeCurrent()
    {
        XRPlayer[] allPlayers = XRPlayer.all;
        foreach (XRPlayer entry in allPlayers)
            entry.isActive = false;

        isActive = true;
        current = this;
        Debug.Log($"Set {this.gameObject.name} as current player.");
    }

    //# Public Methods 
    /// <summary>
    /// Sets given XRPlayer as current player.
    /// </summary>
    public static void SetAsCurrent(XRPlayer player)
    {
        player.MakeCurrent();
    }

    /// <summary>
    /// Finds XRPlayer of given PlayerType in scene and sets it as current player. Returns false if no XRPlayer of this PlayerType could not be found in the current scene.
    /// </summary>
    public static bool SetAsCurrent(PlayerType playerType)
    {
        XRPlayer player = null;
        switch (playerType)
        {
            case PlayerType.HMD: player = FindObjectOfType<HMDPlayer>(true); break;
            case PlayerType.CAVE: player = FindObjectOfType<CAVEPlayer>(true); break;
            default: Debug.LogError($"PlayerType {playerType} cannot be set as current player."); break;
        }

        if (player == null)
        {
            Debug.LogWarning($"Could not find XRPlayer of type {playerType} in scene.");
            return false;
        }
        else
        {
            player.MakeCurrent();
            return true;
        }
    }

    public virtual Pose GetDevicePose(XRDeviceType deviceType)
    {
        switch (deviceType)
            {
                case XRDeviceType.Head: return head.pose;
                case XRDeviceType.LeftHand: return leftHand.pose;
                case XRDeviceType.RightHand: return rightHand.pose;
                default: return Pose.identity;
            }
    }
}
