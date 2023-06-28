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
        // Debug.Log($"Rotation value: {rotate.ReadValue<Vector2>()}");
        // Debug.Log($"Thrust value:   {thrust.ReadValue<float>()}");
        rotateValue = rotate.ReadValue<Vector2>();
        thrustValue = thrust.ReadValue<float>();

        if (mech == null)
            return;

        Move();
        Rotate();

        velocity = mech.velocity;
        angularVelocity = mech.angularVelocity;

        if (mech.position.magnitude > 900) MoveBackToCenter();     //! FOR DEBUGGING PURPOSES ONLY
    }

    private void Move()
    {
        //> MoveTowards Approach
        // mech.position = Vector3.MoveTowards(mech.position, mech.position + mech.forward * thrustValue, maxSpeed);

        //> Position Approach
        // float thrustPower = Mathf.Min(thrustValue * Time.deltaTime, maxSpeed);
        // mech.position += mech.forward * thrustPower;

        //> Rigidbody-based Approach
        mech.AddForce(mech.transform.forward * thrustValue * movementAcceleration, ForceMode.Acceleration);
    }

    private void Rotate()
    {
        Vector3 rotatePower = new Vector3(rotateValue.y, rotateValue.x, 0);
        mech.AddRelativeTorque(rotatePower * rotationSpeed);

        // mech.transform.RotateAround(mech.transform.position, rotatePower, rotationSpeed);
        // mech.transform.Rotate(rotatePower, Space.Self);
    }

    private void OnValidate()
    {
        mech.maxLinearVelocity = maxVelocity;
    }

    private void MoveBackToCenter()     //! FOR DEBUGGING PURPOSES ONLY
    {
        mech.transform.position = Vector3.zero;
    }
}
