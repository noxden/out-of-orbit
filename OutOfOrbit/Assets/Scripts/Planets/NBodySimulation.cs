using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBodySimulation : MonoBehaviour
{
    private static NBodySimulation instance;

    [SerializeField]
    private CelestialBody[] _bodies;
    public static CelestialBody[] bodies
    {
        get
        {
            return instance._bodies;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        _bodies = FindObjectsOfType<CelestialBody>();
        Time.fixedDeltaTime = Universe.physicsTimeStep;
        Debug.Log("Setting fixedDeltaTime to: " + Universe.physicsTimeStep);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < _bodies.Length; i++)
        {
            // Vector3 acceleration = CalculateAcceleration(_bodies[i].Position, _bodies[i]);
            // _bodies[i].UpdateVelocity(acceleration, Universe.physicsTimeStep);
            _bodies[i].UpdateVelocity(_bodies, Universe.physicsTimeStep);
        }

        for (int i = 0; i < _bodies.Length; i++)
        {
            _bodies[i].UpdatePosition(Universe.physicsTimeStep);
        }

    }

    public static Vector3 CalculateAcceleration(Vector3 point, CelestialBody ignoreBody = null)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var body in instance._bodies)
        {
            if (body != ignoreBody)
            {
                float sqrDst = (body.position - point).sqrMagnitude;
                Vector3 forceDir = (body.position - point).normalized;
                acceleration += forceDir * Universe.gravitationalConstant * body.mass / sqrDst;
            }
        }

        return acceleration;
    }
}