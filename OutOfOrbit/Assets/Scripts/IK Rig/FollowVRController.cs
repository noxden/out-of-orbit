using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowVRController : MonoBehaviour
{
   [SerializeField] Transform controller;
   [SerializeField] Vector3 positionOffset, rotationOffset;

   /// <summary>
   /// LateUpdate is called every frame, if the Behaviour is enabled.
   /// It is called after all Update functions have been called.
   /// </summary>

   private void LateUpdate()
   {
        transform.position = Vector3.Lerp(transform.position, controller.TransformPoint(positionOffset), 0.5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, controller.rotation * Quaternion.Euler(rotationOffset), 0.5f);
   }
}
