//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      Project 6 (Grimm, Hausmeier)
// Group:       #6 (Out of Orbit)
// Script by:   Daniel Heilmann (771144)
//================================================================
using System.Collections.Generic;
using UnityEngine;

public enum CelestialBodyType { Planet, Star, Moon, Collected }

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class CelestialBody : GravityObject
{
    //# Inspector 
    public CelestialBodyType type;
    public string bodyName = "Unnamed";
    Transform meshHolder;
    public float radius;
    public float surfaceGravity;
    public Vector3 initialVelocity;

    //# Non-Inspector 
    public Vector3 velocity { get; private set; }
    public float mass { get; private set; }
    public Rigidbody Rigidbody
    {
        get
        {
            return rb;
        }
    }
    public Vector3 position
    {
        get
        {
            return rb.position;
        }
    }

    //# Private Variables 
    private Rigidbody rb;

    void Awake()
    {
        AssignValuesAutomatically();

        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = false;
        velocity = initialVelocity;
    }

    public void UpdateVelocity(CelestialBody[] allBodies, float timeStep)
    {
        foreach (var otherBody in allBodies)
        {
            if (otherBody == this)
                continue;

            float sqrDst = (otherBody.rb.position - rb.position).sqrMagnitude;
            Vector3 forceDir = (otherBody.rb.position - rb.position).normalized;
            Vector3 force = forceDir * Universe.gravitationalConstant * mass * otherBody.mass / sqrDst;
            Vector3 acceleration = force / mass;
            velocity += acceleration * timeStep;
        }
    }

    public void UpdateVelocity(Vector3 acceleration, float timeStep)    //< For if you want to calculate the acceleration in NBodySimulation instead of here
    {
        velocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timeStep)
    {
        rb.MovePosition(rb.position + velocity * timeStep);
    }

    private void AssignValuesAutomatically()
    {
        mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
        meshHolder = GetComponentInChildren<MeshRenderer>().transform;
        // meshHolder = transform.GetChild(0);
        meshHolder.localScale = Vector3.one * radius;
        if (bodyName != null)
            gameObject.name = bodyName;
    }

    void OnValidate()   //< Is called every time any exposed field is modified in the editor
    {
        AssignValuesAutomatically();
    }


}