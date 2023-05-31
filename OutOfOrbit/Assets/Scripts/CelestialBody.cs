using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float independantVelocity;
    public float attractionForce;
    public Vector3 gravitationalPull;
    public Vector3 stick;

    private Transform planetModel;

    private void Start()
    {
        SolarSystemManager.instance.RegisterBody(this);
        planetModel = GetComponentInChildren<Transform>();
    }

    private void LateUpdate()
    {
        transform.right = gravitationalPull;

        transform.position += (gravitationalPull + independantVelocity * transform.forward) * Time.deltaTime;

        //TODO: Remove rotation drift introduced by orbiting
        // RotateAroundSelf();
    }

    public void ApplyAttractionForce(CelestialBody source)
    {
        Vector3 VectorToSource = (source.transform.position - this.transform.position);
        Vector3 direction = VectorToSource.normalized;
        float distance = VectorToSource.magnitude;
        gravitationalPull += direction * Mathf.Clamp(source.attractionForce - this.attractionForce, 0, float.MaxValue);
    }

    private void RotateAroundSelf()
    {
        planetModel.up = stick;
        Debug.DrawLine(transform.position - transform.up * 2, transform.position + transform.up * 2, Color.red, Time.deltaTime);
        Debug.Log($"transform.up is {transform.up}");
        planetModel.RotateAround(transform.position, transform.up, Time.timeSinceLevelLoad * 0.2f * 360f);
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float independantVelocity;
    public float attractionForce;
    public Vector3 gravitationalPull;
    public Vector3 stick;

    private GameObject planetModel;

    private void Start()
    {
        SolarSystemManager.instance.RegisterBody(this);
        planetModel = GetComponentInChildren<Transform>().gameObject;
    }

    private void LateUpdate()
    {
        transform.right = gravitationalPull;

        transform.position += (gravitationalPull + independantVelocity * transform.forward) * Time.deltaTime;

        RotateAroundSelf();
    }

    public void ApplyAttractionForce(CelestialBody source)
    {
        Vector3 VectorToSource = (source.transform.position - this.transform.position);
        Vector3 direction = VectorToSource.normalized;
        float distance = VectorToSource.magnitude;
        gravitationalPull += direction * Mathf.Clamp(source.attractionForce - this.attractionForce, 0, float.MaxValue);
    }

    private void RotateAroundSelf()
    {
        planetModel.transform.rotation = Quaternion.Inverse(this.transform.rotation);

        planetModel.transform.up = stick;
        Debug.DrawLine(planetModel.transform.position - planetModel.transform.up * 2, planetModel.transform.position + planetModel.transform.up * 2, Color.red, Time.deltaTime);
        Debug.Log($"model's transform.up is {planetModel.transform.up}");
        planetModel.transform.RotateAround(planetModel.transform.position, planetModel.transform.up, Time.timeSinceLevelLoad * 0.2f * 360f);
    }
}
*/