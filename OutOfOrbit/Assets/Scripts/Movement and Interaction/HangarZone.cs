using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarZone : MonoBehaviour
{
    [Tooltip("You need to set this manually.")]
    [SerializeField] private Transform mechTransform;

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        AstronautController astronaut = other.GetComponent<AstronautController>();
        if (astronaut)
        {
            astronaut.SetDefaultParent(mechTransform);
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerExit(Collider other)
    {
        AstronautController astronaut = other.GetComponent<AstronautController>();
        if (astronaut)
        {
            astronaut.SetDefaultParent(null);
        }
    }
}
