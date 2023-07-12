using UnityEngine;
using UnityEngine.InputSystem;

public class PawAnimator : MonoBehaviour
{
    private SkinnedMeshRenderer meshRenderer;
    private InputAction gripValueAction;

    private void OnEnable()
    {
        UnityEngine.XR.Interaction.Toolkit.ActionBasedController controller = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.ActionBasedController>();
        if (controller == null) //< Necessary for the NetworkPlayer's hands to not cause log errors
            return;

        gripValueAction = controller.selectActionValue.action;
        // Debug.Log($"{this.name}'s GripValueAction is now \"{gripValueAction.name}\" from \"{gripValueAction.actionMap.name}\".");
        gripValueAction.performed += ReadGripValue;
    }

    private void OnDisable()
    {
        if (gripValueAction != null)
            gripValueAction.performed -= ReadGripValue;
    }

    private void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void ReadGripValue(InputAction.CallbackContext args)
    {
        if (meshRenderer != null)
            SetPawClasp(args.ReadValue<float>());
    }

    public void SetPawClasp(float value)
    {
        meshRenderer.SetBlendShapeWeight(0, value);
    }
}
