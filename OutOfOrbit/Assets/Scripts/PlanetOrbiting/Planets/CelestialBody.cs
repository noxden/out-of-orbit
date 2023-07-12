//================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      Project 6 (Grimm, Hausmeier)
// Group:       #6 (Out of Orbit)
// Script by:   Daniel Heilmann (771144)
//================================================================

using System.Collections.Generic;
using UnityEngine;

public enum CelestialBodyType { Planet, Star, Moon }

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class CelestialBody : GravityObject
{
    [SerializeField] private bool displayGizmos;    //! DEBUG
    [Space(10)]
    //# Inspector 
    public CelestialBodyType type;
    public string bodyName = "Unnamed Celestial Body";
    public Transform meshHolder;
    public float radius;
    public float surfaceGravity;
    public Vector3 initialVelocity;

    //# Non-Inspector 
    public Vector3 velocity { get; private set; }
    public float mass { get; private set; }

    public Rigidbody Rigidbody
    {
        get { return rb; }
    }

    public Vector3 position
    {
        get { return rb.position; }
    }

    //# Private Variables 
    private Rigidbody rb;

    private void Awake()
    {
        AssignValuesAutomatically();

        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = false;
        velocity = initialVelocity;
    }

    public void UpdateVelocity(CelestialBody[] allBodies, float timeStep)
    {
        velocity += CalculateAcceleration(allBodies) * timeStep;
    }

    private Vector3 CalculateAcceleration(CelestialBody[] allBodies)
    {
        Vector3 gravitationalPull = Vector3.zero;
        foreach (var otherBody in allBodies)
        {
            if (otherBody == this)
                continue;

            Vector3 distanceVector = (otherBody.rb.position - rb.position);
            float sqrDst = distanceVector.sqrMagnitude;
            Vector3 forceDir = distanceVector.normalized;
            Vector3 force = forceDir * Universe.gravitationalConstant * mass * otherBody.mass / sqrDst;
            Vector3 acceleration = force / mass;
            gravitationalPull += acceleration;
        }

        return gravitationalPull;
    }

    public void UpdatePosition(float timeStep)
    {
        rb.velocity = velocity;
        // rb.MovePosition(rb.position + velocity * timeStep);
    }

    private void AssignValuesAutomatically()
    {
        mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
        if (meshHolder == null)
            meshHolder = GetComponentInChildren<MeshRenderer>().transform;
        meshHolder.localScale = Vector3.one * radius;
        if (bodyName != null)
            gameObject.name = bodyName;
    }

    private Vector3 CalculateOptimalOrbitVelocity()
    {
        Vector3 gravitationalPull = CalculateAcceleration(NBodySimulation.bodies);
        Vector3 rotationAxis = FindAnyObjectByType<CorrectOrbitChecker>().transform.up;
        Vector3 targetDirection = Quaternion.AngleAxis(90, rotationAxis) * gravitationalPull;
        Vector3 escapeDirection = (targetDirection - gravitationalPull).normalized;
        Vector3 optimalOrbitVelocity = escapeDirection * gravitationalPull.magnitude;

        return optimalOrbitVelocity;
    }

    private void OnValidate() //< Is called every time any exposed field is modified in the editor
    {
        AssignValuesAutomatically();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (!displayGizmos)
            return;
        // Attempted OptimalOrbitVelocity
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position, position + CalculateOptimalOrbitVelocity());

        // Currently applied velocity -> Target for optimal?
        Gizmos.color = Color.green;
        Gizmos.DrawLine(position, position + (rb.velocity - (CalculateAcceleration(NBodySimulation.bodies))));

        // Current rigidbody velocity
        Gizmos.color = Color.black;
        Gizmos.DrawLine(position, position + rb.velocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, position + CalculateAcceleration(NBodySimulation.bodies));
    }

    [ContextMenu("Apply optimal velocity")]
    private void ApplyOptimalVelocity()
    {
        rb.AddForce(CalculateOptimalOrbitVelocity());
    }
}