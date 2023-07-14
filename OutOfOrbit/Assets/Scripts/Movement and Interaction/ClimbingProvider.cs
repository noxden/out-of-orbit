using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// We average the velocity of the controllers and move the origin via a Character Controller or transform.
/// </summary>
public class ClimbingProvider : LocomotionProvider
{
    private Transform origin;
    public AstronautController controller;
    public bool isClimbing { get; private set; } = false;
    private List<VelocityContainer> activeVelocities = new List<VelocityContainer>();

    protected override void Awake()
    {
        base.Awake();
        FindCharacterController();
    }

    private void FindCharacterController()
    {
        if (!origin)
            origin = system.xrOrigin.transform;
    }

    public void AddProvider(VelocityContainer provider)
    {
        if (!activeVelocities.Contains(provider))
        {
            activeVelocities.Add(provider);
        }
    }

    public void RemoveProvider(VelocityContainer provider)
    {
        if (activeVelocities.Contains(provider))
        {
            activeVelocities.Remove(provider);
        }
    }



    private void Update()
    {
        TryBeginClimb();
        if (isClimbing)
        {
            ApplyVelocity();
        }

        TryEndClimb();
    }

    private void TryBeginClimb()    // TODO: Invoke events here to notify other classes that climbing has started / stopped
    {
        if (CanClimb() && BeginLocomotion())
        {
            isClimbing = true;
        }
    }

    private void TryEndClimb()
    {
        if (!CanClimb() && EndLocomotion())
        {
            isClimbing = false;
        }
    }

    private bool CanClimb()
    {
        return activeVelocities.Count != 0;
    }

    private void ApplyVelocity()
    {
        Vector3 velocity = CollectControllerVelocity();

        velocity = origin.TransformDirection(velocity);
        velocity *= Time.deltaTime;

        origin.localPosition -= velocity;
    }

    public Vector3 CollectControllerVelocity()
    {
        Vector3 totalVelocity = Vector3.zero;
        foreach (VelocityContainer container in activeVelocities)
            totalVelocity += container.Velocity;
        return totalVelocity;
    }
}