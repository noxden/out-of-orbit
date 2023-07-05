using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectOrbitChecker : MonoBehaviour
{
    private BoxCollider boxCollider;
    [SerializeField] private float orbitRadiusOutside;
    private float orbitRadiusInside;
    [SerializeField] private float orbitThicknessHorizontal;
    [SerializeField] private float orbitThicknessVertical;
    [SerializeField] private List<OutOfOrbitPlanet> collidingPlanets = new List<OutOfOrbitPlanet>();

    private void Start()
    {
        orbitRadiusInside = orbitRadiusOutside - orbitThicknessHorizontal;
        if (boxCollider == null)
            boxCollider = this.gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(orbitRadiusOutside * 2, orbitThicknessVertical, orbitRadiusOutside * 2);
    }

    private void Update()
    {
        if (collidingPlanets.Count == 0)
            return;

        foreach (OutOfOrbitPlanet planet in collidingPlanets)
        {
            float distanceOfPlanetToOrbitCenter = Vector3.Distance(transform.position, planet.transform.position);
            if (isWithinRange(value: distanceOfPlanetToOrbitCenter, rangeMin: orbitRadiusInside, rangeMax: orbitRadiusOutside))
            {
                planet.consecutiveTimeInCorrectOrbit += Time.deltaTime;
                // Debug.Log($"Added {Time.deltaTime} to {planet.name}'s consecutiveTimeInCorrectOrbit.");
            }
            else
                planet.consecutiveTimeInCorrectOrbit = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"{other.name} has entered the orbit checker.");
        OutOfOrbitPlanet planet = other.GetComponentInParent<OutOfOrbitPlanet>();
        if (planet != null)
            collidingPlanets.Add(planet);
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log($"{other.name} has left the orbit checker.");
        OutOfOrbitPlanet planet = other.GetComponentInParent<OutOfOrbitPlanet>();
        if (planet != null)
            collidingPlanets.Remove(planet);
    }

    private void OnDrawGizmos()
    {
        foreach (OutOfOrbitPlanet planet in collidingPlanets)
        {
            Gizmos.color = Color.red;
            float distanceOfPlanetToOrbitCenter = Vector3.Distance(transform.position, planet.transform.position);
            if (distanceOfPlanetToOrbitCenter < orbitRadiusOutside)
            {
                Gizmos.color = Color.yellow;
                if (distanceOfPlanetToOrbitCenter > orbitRadiusInside)
                {
                    Gizmos.color = Color.green;
                    // Debug.Log($"{planet.name} is in the correct orbit.");
                }
            }
            // string isGood = $"{(distanceOfPlanetToOrbitCenter < orbitRadiusInside ? "too close" : (distanceOfPlanetToOrbitCenter > orbitRadiusOutside ? "too far away" : "perfectly in range"))}";
            // Debug.Log($"{planet.name} is {distanceOfPlanetToOrbitCenter} away from center. Needs to be between {orbitRadiusInside} and {orbitRadiusOutside}. -> Is {isGood}");
            Gizmos.DrawLine(transform.position, planet.transform.position);
        }
    }

    private bool isWithinRange(float value, float rangeMin, float rangeMax) => (value > rangeMin && value < rangeMax);
}
