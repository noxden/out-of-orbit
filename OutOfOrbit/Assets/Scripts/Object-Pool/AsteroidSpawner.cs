using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawning : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public float probability = 4.0f;
    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0, 100) < probability)
        {
            GameObject mAsteroid = Pool.singleton.Get("asteroid");
            if (mAsteroid != null)
            {
                mAsteroid.transform.position = this.transform.position
                        + new Vector3(Random.Range(-10, 10), 0, 0);
                mAsteroid.transform.localScale = new Vector3(Random.Range(0.3f, 2.0f), Random.Range(0.5f, 3.0f), 1.0f);
                mAsteroid.SetActive(true);
            }
        }
    }
}
