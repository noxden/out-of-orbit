using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnInvis : MonoBehaviour
{   
    public float maxDistance = 70f;
    public GameObject centerPrefab;
    public Vector3 center;
    public Vector3 asteroidPos;

    public void Update() 
    {
        centerPrefab.transform.position = center;
        Vector3 distanceFromCenterToAsteroid = asteroidPos - center;

        if(distanceFromCenterToAsteroid.magnitude > maxDistance) 
        {
            this.gameObject.SetActive(false);
        }
    }

}
