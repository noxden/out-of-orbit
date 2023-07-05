using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfOrbitPlanet : MonoBehaviour
{
    [SerializeField] private bool lockedInCorrectOrbit = false;
    [SerializeField] private float requiredConsecutiveTimeInCorrectOrbit = 0f;
    public float consecutiveTimeInCorrectOrbit
    {
        get
        {
            return m_consecutiveTimeInCorrectOrbit;
        }
        set
        {
            m_consecutiveTimeInCorrectOrbit = value;
            CheckTimeInCorrectOrbit();
        }
    }
    [SerializeField] private float m_consecutiveTimeInCorrectOrbit;


    public void CheckTimeInCorrectOrbit()
    {
        if (lockedInCorrectOrbit)
            return;
            
        if (consecutiveTimeInCorrectOrbit > requiredConsecutiveTimeInCorrectOrbit)
        {
            Debug.Log($"Locked {this.name} into correct orbit. Good job!");
            lockedInCorrectOrbit = true;
        }
    }
}
