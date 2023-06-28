using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionLimiter : MonoBehaviour
{
    [SerializeField] private float bounds = 3f;

    private void Update()
    {
        transform.localPosition = UniformVectorClamp(transform.localPosition, -bounds, bounds); //< To keep the camera from going out of bounds
    }

    private Vector3 UniformVectorClamp(Vector3 input, float min, float max)
    {
        Vector3 output;
        output.x = Mathf.Clamp(input.x, min, max);
        output.y = input.y;
        output.z = Mathf.Clamp(input.z, min, max);
        return output;
    }
}
