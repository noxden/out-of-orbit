using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfOrbitPlanet : MonoBehaviour
{
    public bool lockedInCorrectOrbit = false;
    [SerializeField] private float requiredConsecutiveTimeInCorrectOrbit = 0f;

    public float consecutiveTimeInCorrectOrbit
    {
        get { return m_consecutiveTimeInCorrectOrbit; }
        set
        {
            m_consecutiveTimeInCorrectOrbit = value;
            CheckTimeInCorrectOrbit();
        }
    }

    [SerializeField] private float m_consecutiveTimeInCorrectOrbit;


    public void CheckTimeInCorrectOrbit()
    {
        if (consecutiveTimeInCorrectOrbit >= requiredConsecutiveTimeInCorrectOrbit)
        {
            if (lockedInCorrectOrbit) return;

            Debug.Log($"Locked {this.name} into correct orbit. Good job!");
            lockedInCorrectOrbit = true;
        }
        else
        {
            lockedInCorrectOrbit = false;
        }
    }
}