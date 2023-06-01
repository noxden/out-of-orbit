using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.DrawLine(transform.position + transform.up, transform.position - transform.up, Color.white, Universe.physicsTimeStep);
        transform.RotateAround(transform.position, transform.up, Universe.physicsTimeStep * rotationsPerSecond * 360f);
    }
}
