using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PawAnimator : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private InputAction gripValueAction;
    private float gripValue => gripValueAction.ReadValue<float>();

    private void Awake()
    {
        UnityEngine.XR.Interaction.Toolkit.ActionBasedController controller = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.ActionBasedController>();
        if (controller != null)
            gripValueAction = controller.selectActionValue.action;

        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        if (gripValueAction == null) //< Necessary for the NetworkPlayer's hands to not cause log errors
            return;

        SetPawClasp(gripValue);
    }

    public void SetPawClasp(float value)
    {
        skinnedMeshRenderer.SetBlendShapeWeight(0, value * 100); //< Multiplier is necessary because I messed up the export of the blendshapes so now they range from 0 to 100.
    }
}