using UnityEngine;

public class CelestialBody_deprecated : MonoBehaviour
{
    public GameObject orbitsAround;
    public float orbitingSpeed = 1f;
    public float distance = 1f;
    public Quaternion orbitingAngle;
    public float rotationsPerSecond = 1f;
    public Vector3 inclination;
    private float timeSinceOrbitStart = 0f;
    private GameObject realOrbitingCenter = null;

    public static CelestialBody_deprecated Construct(GameObject AttachComponentToThis, GameObject orbitsAround, float speed, float distance, Quaternion orbitingAngle)
    {
        CelestialBody_deprecated component = AttachComponentToThis.AddComponent<CelestialBody_deprecated>();
        component.orbitsAround = orbitsAround;
        component.orbitingSpeed = speed;
        component.distance = distance;
        component.orbitingAngle = orbitingAngle;
        return component;
    }

    private void Start()
    {
        if (orbitsAround == null)
        {
            Debug.LogWarning($"{this.name} does not have an orbiting center.");
            return;
        }

        realOrbitingCenter = new GameObject($"RotationOrigin of {this.name}");
        realOrbitingCenter.transform.SetParent(orbitsAround.transform);
        transform.SetParent(realOrbitingCenter.transform);
    }

    private void LateUpdate()
    {
        if (orbitsAround != null)
            Orbit();

        // RotateAroundSelf();
    }

    private void Orbit()
    {
        realOrbitingCenter.transform.localRotation = orbitingAngle;
        // this.transform.localRotation = Quaternion.Inverse(orbitingAngle);

        ////////////////////////////////

        timeSinceOrbitStart += Time.deltaTime;

        Vector3 newPos;
        newPos = new Vector3(Mathf.Sin(timeSinceOrbitStart * (orbitingSpeed / distance)) * distance, 0, Mathf.Cos(timeSinceOrbitStart * (orbitingSpeed / distance)) * distance);

        transform.localPosition = newPos;
    }

    private void RotateAroundSelf()
    {
        transform.RotateAround(transform.position, transform.up, timeSinceOrbitStart * rotationsPerSecond * 360f);
    }
}
