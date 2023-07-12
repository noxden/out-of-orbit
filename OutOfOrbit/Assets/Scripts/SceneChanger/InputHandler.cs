using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionReference thrustActionReference;
    [SerializeField]
    private List<InputActionReference> grabActionReferences;
    [SerializeField]
    private UnityEvent DoSceneChange;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        switch (Settings.defaultXRPlayer)
        {
            case PlayerType.CAVE:
                thrustActionReference.action.Enable();
                thrustActionReference.action.performed += OnActionPerformed;
                break;
            case PlayerType.HMD:
                foreach (InputActionReference entry in grabActionReferences)
                {
                    entry.action.Enable();
                    entry.action.performed += OnActionPerformed;
                }
                break;
        }

    }

    private void OnDisable()
    {
        // switch (Settings.defaultXRPlayer)    //< We don't want to take away the thrust or grab action from the players in the case that we disable the InputHandler under some circumstances.
        // {
        //     case PlayerType.CAVE:
        //         thrustActionReference.action.Disable();
        //         thrustActionReference.action.performed -= OnActionPerformed;
        //         break;
        //     case PlayerType.HMD:
        //         foreach (InputActionReference entry in grabActionReferences)
        //         {
        //             entry.action.Disable();
        //             entry.action.performed -= OnActionPerformed;
        //         }
        //         break;
        // }

    }

    public void OnActionPerformed(InputAction.CallbackContext args)
    {
        if (args.ReadValueAsButton())
        {
            DoSceneChange.Invoke();
        }
    }
}
