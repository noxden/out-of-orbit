using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private bool GizmoMode = false;
    [SerializeField] private int cubeAmount;
    [SerializeField] private int rangeMin;
    [SerializeField] private int rangeMax;

    [SerializeField] private List<Vector3> cubePositions = new List<Vector3>();

    private void Start()
    {
        for (int i = 0; i < cubeAmount; i++)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (GizmoMode)
            cubePositions.Add(RandomPosition());
        else
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = RandomPosition();
            go.transform.localScale = Vector3.one * 3;
            Color positionBasedColor = new Color(r: go.transform.position.x / rangeMax, g: go.transform.position.y / rangeMax, b: go.transform.position.z / rangeMax);
            go.GetComponent<Renderer>().material.color = positionBasedColor;
            go.GetComponent<Renderer>().material.SetColor("_EmissionColor", positionBasedColor);
            go.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
            DestroyImmediate(go.GetComponent<Collider>());
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 position in cubePositions)
        {
            Gizmos.color = new Color(r: position.x / rangeMax, g: position.y / rangeMax, b: position.z / rangeMax);
            Gizmos.DrawCube(position, Vector3.one * 3);
        }
    }

    private Vector3 RandomPosition()
    {
        Vector3 randomPos = Vector3.zero;
        do
            randomPos = new Vector3(Random.Range(-rangeMax, rangeMax), Random.Range(-rangeMax, rangeMax), Random.Range(-rangeMax, rangeMax));
        while (randomPos.magnitude > rangeMax);

        if (randomPos.magnitude <= rangeMin)
            return RandomPosition();

        return randomPos;
    }
}