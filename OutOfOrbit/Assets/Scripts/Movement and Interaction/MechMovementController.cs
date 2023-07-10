using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechMovementController : MonoBehaviour
{
    [Header("Manually assigned fields")]
    [SerializeField] private Rigidbody mech;
    [SerializeField] private InputActionReference rotateInput;
    [SerializeField] private InputActionReference thrustInput;

    [Header("Tweakable Movement Settings")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float rotationSpeed;

    [Header("Rigidbody Visualization")]
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 angularVelocity;

    [Header("Input Visualization")]
    [SerializeField] private Vector2 rotateValue;
    [SerializeField] private float thrustValue;

    private InputAction rotate;
    private InputAction thrust;

    private void Awake()
    {
        rotate = rotateInput.action;
        thrust = thrustInput.action;
    }

    private void OnEnable()
    {
        rotate.Enable();
        thrust.Enable();
    }
    private void OnDisable()
    {
        rotate.Disable();
        thrust.Disable();
    }

    void Start()
    {
        mech.maxLinearVelocity = maxVelocity;
    }

    private void FixedUpdate()
    {
        if (mech == null)
            return;

        if (rotate.inProgress)
        {
            rotateValue = rotate.ReadValue<Vector2>();
            Rotate();
        }
        if (thrust.inProgress)
        {
            thrustValue = thrust.ReadValue<float>();
            Move();
        }

        //> For visualization purposes
        velocity = mech.velocity;
        angularVelocity = mech.angularVelocity;

        if (mech.position.magnitude > 900) mech.transform.position = Vector3.zero;  //< If too far away, move back to center   //! FOR DEBUGGING PURPOSES ONLY
    }

    private void Move()
    {
        //> Rigidbody-based Approach
        mech.AddForce(mech.transform.forward * thrustValue * movementAcceleration, ForceMode.Acceleration);
    }

    private void Rotate()
    {
        Vector3 rotatePower = new Vector3(rotateValue.y, rotateValue.x, 0);
        mech.AddRelativeTorque(rotatePower * rotationSpeed);
    }

    private void OnValidate()
    {
        mech.maxLinearVelocity = maxVelocity;
    }
}
