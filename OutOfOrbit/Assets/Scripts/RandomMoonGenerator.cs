using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoonGenerator : MonoBehaviour
{
    public int minAmount = 1;
    public int maxAmount = 1;
    [Space(5)]
    public float minSpeed = 1f;
    public float maxSpeed = 1f;
    [Space(5)]
    public float minDistance = 1f;
    public float maxDistance = 1f;
    [Space(5)]
    public float minScale = 1f;
    public float maxScale = 1f;
    [Space(5)]
    public bool uniformOrbitingAngle;

    [Tooltip("For visualization purposes only. Do not modify manually!")]
    public List<GameObject> generatedMoons;

    private void Start()
    {
        if (maxAmount == 0)
        {
            Debug.LogWarning($"Max Amount is 0. Did not generate any moons.");
            return;
        }

        if (minAmount > maxAmount)
        {
            Debug.LogWarning($"Max Amount ({maxAmount}) is bigger than Min Amount ({minAmount}), dummy.");
            return;
        }

        Quaternion randomAngle = Quaternion.Euler(0, 0, 0);
        if (uniformOrbitingAngle)
            randomAngle = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        int randomMoonAmount = Random.Range(minAmount, maxAmount + 1);
        Debug.Log($"Generated {randomMoonAmount} moons!");

        for (int i = 0; i < randomMoonAmount; i++)
        {
            GameObject newMoon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float randomSpeed = Random.Range(minSpeed, maxSpeed);
            float randomDistance = Random.Range(minDistance, maxDistance);
            if (!uniformOrbitingAngle)
                randomAngle = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            CelestialBody.Construct(newMoon, this.gameObject, randomSpeed, randomDistance, randomAngle);
            newMoon.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
            generatedMoons.Add(newMoon);
        }
    }
}
