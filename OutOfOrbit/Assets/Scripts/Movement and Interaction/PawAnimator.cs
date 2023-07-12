using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PawAnimator : MonoBehaviour
{
    private SkinnedMeshRenderer meshRenderer;
    private InputAction gripValueAction;
    private float gripValue => gripValueAction.ReadValue<float>();

    private void Awake()
    {
        UnityEngine.XR.Interaction.Toolkit.ActionBasedController controller = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.ActionBasedController>();
        gripValueAction = controller.selectActionValue.action;
    }

    private void Update()
    {
        if (gripValueAction == null) //< Necessary for the NetworkPlayer's hands to not cause log errors
            return;
        
        SetPawClasp(gripValue);
    }

    public void SetPawClasp(float value)
    {
        meshRenderer.SetBlendShapeWeight(0, value * 100); //< Multiplier is necessary because I messed up the export of the blendshapes so now they range from 0 to 100.
    }
}