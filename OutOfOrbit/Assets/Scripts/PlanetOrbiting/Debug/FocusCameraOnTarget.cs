using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FocusCameraOnTarget : MonoBehaviour
{
    [Tooltip("Is automatically assigned to OrbitDebugDisplay's centralBody if not set manually.")]
    public Transform target;
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null)
        {
            target = FindObjectOfType<OrbitDebugDisplay>().centralBody.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        cam.transform.LookAt(target);

        Vector3 dirToTarget = (target.position - this.transform.position).normalized;
        transform.localPosition = new Vector3(-dirToTarget.x * cameraDistance, cameraHeight, -dirToTarget.z * cameraDistance);
    }
}
