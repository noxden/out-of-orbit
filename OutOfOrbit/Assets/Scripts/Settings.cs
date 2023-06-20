using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VRType { HMD, CAVE }

public class Settings : MonoBehaviour
{
    //# Debug Flags 
    public static bool showOwnedNetworkPlayer = true;
    public static VRType VRType = VRType.CAVE;
}
