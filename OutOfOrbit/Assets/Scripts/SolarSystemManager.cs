using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    public static SolarSystemManager instance { get; set; }

    public List<CelestialBody> celestialBodies;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // private void Update()
    // {
    //     foreach (CelestialBody body in celestialBodies)
    //     {
    //         body.gravitationalPull = Vector3.zero;
    //     }
        
    //     foreach (CelestialBody bodyA in celestialBodies)
    //     {
    //         foreach (CelestialBody bodyB in celestialBodies)
    //         {
    //             if (bodyA != bodyB)
    //             {
    //                 bodyB.ApplyAttractionForce(bodyA);
    //             }
    //         }
    //     }
    // }

    public void RegisterBody(CelestialBody body) => celestialBodies.Add(body);
    public void UnregisterBody(CelestialBody body) => celestialBodies.Remove(body);
}
