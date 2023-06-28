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

        SetCurrentPlayerBasedOnSettings();
    }

    private void SetCurrentPlayerBasedOnSettings()
    {
        XRPlayer.SetAsCurrent(Settings.defaultXRPlayer);
    }
}
