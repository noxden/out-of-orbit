using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillateUpDown : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        this.transform.position += Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad * 0.5f) * Time.deltaTime;
    }
}
