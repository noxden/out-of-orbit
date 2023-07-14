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
        {
            DestroyImmediate(this);
            instance = this;
        }

        DisplayCountChecker();
        SetCurrentPlayerBasedOnSettings();
    }

    private void SetCurrentPlayerBasedOnSettings()
    {
        XRPlayer.SetAsCurrent(Settings.defaultXRPlayer);
    }

    private void DisplayCountChecker()
    {
        int displayCount = Display.displays.Length;
        Debug.Log($"{displayCount} Display{(displayCount > 1 ? "s" : "")} connected: Trying to set default player to {(displayCount >= 7 ? "CAVE" : "HMD")}.");
        if (displayCount >= 7)
            Settings.defaultXRPlayer = PlayerType.CAVE;
        else
            Settings.defaultXRPlayer = PlayerType.HMD;
    }
}
