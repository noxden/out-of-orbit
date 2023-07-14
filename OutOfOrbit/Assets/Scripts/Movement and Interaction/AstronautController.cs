using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AstronautController : MonoBehaviour
{
    [SerializeField] private InputActionProperty thrustInput;
    private InputAction thrustAction;
    private Vector2 thrustValue
    {
        get
        {
            if (thrustAction.inProgress) return thrustAction.ReadValue<Vector2>();
            else return Vector2.zero;
        }
    }
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float maxMovementVelocity;

    [Header("These fields should assign themselves during play mode")]
    [SerializeField] private ClimbingProvider provider;
    [SerializeField] private CharacterController character;
    [SerializeField] private Rigidbody body;

    private bool m_isAttached;
    public bool isAttached
    {
        get => m_isAttached;
        set
        {
            m_isAttached = value;
            if (value == true)
                body.velocity = Vector3.zero;
            body.isKinematic = value;
        }
    }

    private List<Transform> attachedClimbAnchors = new List<Transform>();

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        FindComponents();
        thrustAction = thrustInput.action;
    }

    private void OnEnable()
    {
        thrustAction.Enable();
    }
    private void OnDisable()
    {
        thrustAction.Disable();
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        if (!isAttached) ApplyThrustForce();
    }

    private void ApplyThrustForce()
    {
        if (body.velocity.magnitude < maxMovementVelocity)
            body.AddForce(body.transform.forward * thrustValue.y * movementAcceleration, ForceMode.Acceleration);
    }

    public void AttachToAnchor(ClimbAnchor anchor)
    {
        Transform anchorTransform = anchor.transform;
        Debug.Log($"Attaching CharacterController {character.name} to ClimbAnchor ({anchor.name}).");

        attachedClimbAnchors.Add(anchorTransform);

        character.gameObject.transform.SetParent(attachedClimbAnchors[attachedClimbAnchors.Count - 1], worldPositionStays: true);
        isAttached = true;
    }

    public void DetachFromAnchor(ClimbAnchor anchor)
    {
        Transform anchorTransform = anchor.transform;
        Debug.Log($"Detaching CharacterController {character.name} from ClimbAnchor ({anchor.name}).");

        attachedClimbAnchors.Remove(anchorTransform);
        if (attachedClimbAnchors.Count == 0)
        {
            Vector3 lastVelocity = provider.CollectControllerVelocity();

            character.gameObject.transform.SetParent(defaultParent, worldPositionStays: true);
            isAttached = false;
            body.AddForce(-lastVelocity * 100);
        }
        else
            character.gameObject.transform.SetParent(attachedClimbAnchors[attachedClimbAnchors.Count - 1], worldPositionStays: true);
    }

    private void FindComponents()
    {
        if (!character)
            character = GetComponent<CharacterController>();

        if (!body)
            body = GetComponent<Rigidbody>();

        if (!provider)
            provider = FindAnyObjectByType<ClimbingProvider>();
    }
}
