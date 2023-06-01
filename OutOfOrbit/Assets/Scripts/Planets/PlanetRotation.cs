using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public float rotationsPerSecond = 1f;

    private void FixedUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        Debug.DrawLine(transform.position + transform.up, transform.position - transform.up, Color.white, Universe.physicsTimeStep);
        transform.RotateAround(transform.position, transform.right, Universe.physicsTimeStep * rotationsPerSecond * 360f);
    }
}
