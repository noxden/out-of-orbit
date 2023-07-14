using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnInvis : MonoBehaviour
{
    public float maxDistance = 150f;
    public Transform center;
    public float initialVelocity;

    public Transform HMDPlayerTransform;
    private Rigidbody myRigidbody;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        HMDPlayer player = FindObjectOfType<HMDPlayer>();
        HMDPlayerTransform = player.transform;
    }
    private void Start()
    {
        center = FindObjectOfType<AsteroidSpawner>().center;
    }

    private void Update()
    {
        WhenToDiactivate();
        // LaunchAtPlayer();

    }

   /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        initialVelocity = Random.Range(60f, 300f);
        myRigidbody.AddForce((HMDPlayerTransform.position - transform.position).normalized * initialVelocity);
    }

    private void WhenToDiactivate()
    {
        Vector3 distanceFromCenterToAsteroid = this.transform.position - center.position;
        if (distanceFromCenterToAsteroid.magnitude > maxDistance)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void LaunchAtPlayer()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, HMDPlayerTransform.position, initialVelocity);
    }

}
