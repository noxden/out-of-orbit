using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyStats : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 angularVelocity;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;
            
        this.velocity = rb.velocity;
        this.angularVelocity = rb.angularVelocity;
    }

[ContextMenu("Stop Velocity Immediately")]
    private void StopVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
