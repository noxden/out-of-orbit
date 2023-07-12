using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public float probability = 4.0f;
    public Transform center;
    // Update is called once per frame

    public void Update()
    {
        if (Random.Range(0, 100) < probability)
        {
            GameObject mAsteroid = AsteroidPool.singleton.Get("asteroid");
            if (mAsteroid != null)
            {
                mAsteroid.transform.position = this.transform.position
                        + new Vector3(Random.Range(-30, 30), 0, 0);
                mAsteroid.SetActive(true);
            }
        }
    }
}
