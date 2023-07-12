using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetRotation : MonoBehaviour
{
    public Vector3 rotationAxisTilt;
    public float rotationsPerSecond = 1f;

    private void Start()
    {
        transform.localRotation = Quaternion.Euler(rotationAxisTilt);
    }

    private void FixedUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.RotateAround(transform.position, transform.up, Universe.physicsTimeStep * rotationsPerSecond * 360f);
    }

    private void OnValidate()
    {
        transform.localRotation = Quaternion.Euler(rotationAxisTilt);
    }

    // private void OnDrawGizmos()
    // {
    //     Vector3 lineLength = transform.up * transform.localScale.magnitude * 0.5f;
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawLine(transform.position + lineLength, transform.position - lineLength);
    // }
}
