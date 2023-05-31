//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      Project 6 (Grimm, Hausmeier)
// Group:       #6 (Out of Orbit)
// Script by:   Daniel Heilmann (771144)
//================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    //# Orbiting 
    [Header("Orbiting Around Center")]
    public Transform orbitCenter;
    public float orbitDistance;
    public float orbitingSpeed = 1f;
    public Quaternion orbitAngle;

    //# Rotating 
    [Header("Rotating Around Self")]
    public float rotationsPerSecond = 1f;
    public Vector3 rotationInclination;

    //# Private Variables 
    private float timeSinceOrbitStart;
    private Vector3 newPos;
    private GameObject planetModel;

    //# Monobehaviour Methods
    private void Start()
    {
        planetModel = GetComponentInChildren<MeshRenderer>().gameObject;

        if (orbitCenter == null)
        {
            Debug.LogWarning($"{this.name} does not have an orbiting center assigned.");
            return;
        }

        // GameObject emptyOrbitingCenterObject = new GameObject($"OrbitingCenter of {this.name}");
        // emptyOrbitingCenterObject.transform.SetParent(orbitCenter);
        // orbitCenter = emptyOrbitingCenterObject.transform;  //< Reassign orbitCenter to newly created gameobject
        // transform.SetParent(orbitCenter);
    }

    private void LateUpdate()
    {
        if (orbitCenter == null)
            return;

        // orbitCenter.localRotation = orbitAngle;
        Orbit();
        transform.position = Vector3.MoveTowards(transform.position, newPos, orbitingSpeed);
        // Rotate();
    }

    //# Private Methods 
    private void Orbit()
    {
        timeSinceOrbitStart += Time.deltaTime;
        float mathInput = timeSinceOrbitStart * (orbitingSpeed / orbitDistance);

        newPos = orbitCenter.position + new Vector3(Mathf.Sin(mathInput) * orbitDistance, 0, Mathf.Cos(mathInput) * orbitDistance);
        Debug.Log($"newPos is {newPos}");
    }

    private void Rotate()
    {
        // transform.localRotation = Quaternion.Inverse(orbitAngle);
        // planetModel.transform.Rotate(new Vector3(0, 0, 360f * rotationsPerSecond), Space.Self);
        transform.RotateAround(transform.position, transform.up, timeSinceOrbitStart * rotationsPerSecond * 360f);
    }
}