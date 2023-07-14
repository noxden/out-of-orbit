using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Responsible for getting the velocity container from the controller to the climbing provider so it can be processed.
/// </summary>
public class ClimbAnchor : XRBaseInteractable
{
    [SerializeField] private ClimbingProvider climbingProvider;
    protected override void Awake()
    {
        base.Awake();
        FindClimbingProvider();
    }

    private void Start()
    {
        if (this.transform.localScale != Vector3.one)
            Debug.LogWarning($"Watch out! The ClimbingAnchor \"{this.name}\" is on a scaled gameobject. This will cause your view to become distorted when holding onto it. Create an empty parent instead and put the ClimbingAnchor component on that.");
    }

    private void FindClimbingProvider()
    {
        if (!climbingProvider)
        {
            climbingProvider = FindObjectOfType<ClimbingProvider>();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        TryAdd(args.interactorObject);
    }

    private void TryAdd(IXRSelectInteractor interactor)
    {
        if (interactor.transform.TryGetComponent(out VelocityContainer container))
        {
            climbingProvider.AddProvider(container);
            climbingProvider.controller.AttachToAnchor(this);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        TryRemove(args.interactorObject);
    }

    private void TryRemove(IXRSelectInteractor interactor)
    {
        if (interactor.transform.TryGetComponent(out VelocityContainer container))
        {
            climbingProvider.controller.DetachFromAnchor(this);  //< Needs to be called before removing the hand (VelocityContainer) from the ClimbingProvider in order to allow calculation of lastVelocity.
            climbingProvider.RemoveProvider(container);
        }
    }

    public override bool IsHoverableBy(IXRHoverInteractor interactor)
    {
        return base.IsHoverableBy(interactor) && interactor is XRDirectInteractor;
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        return base.IsSelectableBy(interactor) && interactor is XRDirectInteractor;
    }
}
