using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    //# Debug Flags 
    public static bool showOwnedNetworkPlayer = true;
    private static PlayerType? m_defaultXRPlayer;   //< Set this value manually in this script to overwrite automatic player type selection.
    public static PlayerType defaultXRPlayer
    {
        set
        {
            if (m_defaultXRPlayer.HasValue)
                Debug.LogWarning($"Default player has been set to {m_defaultXRPlayer.Value} manually in Settings. If you wish to allow for automatic selection of the default player, please remove the value of m_defaultXRPlayer within the script.");
            else
                m_defaultXRPlayer = value;
        }
        get
        {
            if (!m_defaultXRPlayer.HasValue)
                Debug.LogWarning($"Default player has not been set yet, returning default value ({m_defaultXRPlayer.GetValueOrDefault()}).");
            return m_defaultXRPlayer.GetValueOrDefault();
        }
    }
    public bool isOverwritingDefaultXRPlayer { get { return m_defaultXRPlayer.HasValue; } }
}
