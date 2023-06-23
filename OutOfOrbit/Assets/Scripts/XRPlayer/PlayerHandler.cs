using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-101)]
public class PlayerHandler : MonoBehaviour
{
    public static PlayerHandler instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(this);

        EnableCorrespondingPlayer();
    }

    private void EnableCorrespondingPlayer()
    {
        XRPlayer.SetAsCurrent(Settings.defaultXRPlayer);

        //> Initial implementation
        // XRPlayer HMDPlayer = FindObjectOfType<HMDPlayer>(true); //< Allows the system to ALWAYS enable the player, even if the XRPlayer component on the gameobject is disabled.
        // XRPlayer CAVEPlayer = FindObjectOfType<CAVEPlayer>(true);

        // if (!CAVEPlayer || !HMDPlayer)
        //     Debug.LogWarning($"Could not find the following XRPlayer in scene: {(!CAVEPlayer ? "CAVEPlayer" : "")}{(!CAVEPlayer && !HMDPlayer ? ", " : "")}{(!HMDPlayer ? "HMDPlayer" : "")}");

        // switch (Settings.playMode)
        // {
        //     case PlayerType.HMD: HMDPlayer.MakeCurrent(); break;
        //     case PlayerType.CAVE: CAVEPlayer.MakeCurrent(); break;
        //     default:
        //         Debug.LogError($"Playmode {Settings.playMode} is neither HMD nor CAVE. This should not be possible, something must have gone wrong.", this);
        //         break;
        // }
    }
}
