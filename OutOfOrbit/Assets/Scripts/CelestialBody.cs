using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public GameObject orbitsAround;
    public float speed = 1f;
    public float distance = 1f;
    public Quaternion orbitingAngle;

    private float timeSinceOrbitStart = 0f;
    private GameObject rotationEmpty = null;

    public static CelestialBody Construct(GameObject AttachComponentToThis, GameObject orbitsAround, float speed, float distance, Quaternion orbitingAngle)
    {
        CelestialBody component = AttachComponentToThis.AddComponent<CelestialBody>();
        component.orbitsAround = orbitsAround;
        component.speed = speed;
        component.distance = distance;
        component.orbitingAngle = orbitingAngle;
        return component;
    }

    private void Start()
    {
        if (orbitsAround == null)
            Debug.LogWarning($"{this.name} does not have an orbiting center.");

        rotationEmpty = new GameObject($"RotationOrigin of {this.name}");
        rotationEmpty.transform.SetParent(orbitsAround.transform);
        transform.SetParent(rotationEmpty.transform);
    }

    private void Update()
    {
        rotationEmpty.transform.localRotation = orbitingAngle;
        this.transform.localRotation = Quaternion.Inverse(orbitingAngle);

        ////////////////////////////////

        timeSinceOrbitStart += Time.deltaTime * (speed / distance);

        Vector3 newPos;
        newPos = new Vector3(Mathf.Sin(timeSinceOrbitStart) * distance, 0, Mathf.Cos(timeSinceOrbitStart) * distance);

        transform.localPosition = newPos;
    }
}
