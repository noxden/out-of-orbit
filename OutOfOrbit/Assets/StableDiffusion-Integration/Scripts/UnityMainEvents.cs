using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityMainEvents : MonoBehaviour
{
    public UnityEvent OnStart;
    public UnityEvent OnUpdate;
    // Start is called before the first frame update
    void Start()
    {
        OnStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate?.Invoke();
    }
}
