using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode { HMD, CAVE }

public class Settings : MonoBehaviour
{
    //# Debug Flags 
    public static bool showOwnedNetworkPlayer = true;
    public static PlayMode playMode = PlayMode.CAVE;
}
