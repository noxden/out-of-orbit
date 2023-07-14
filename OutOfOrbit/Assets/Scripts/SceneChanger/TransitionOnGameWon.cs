using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransitionOnGameWon : MonoBehaviour
{
    [SerializeField] float waitTime = 0.0f;
    public UnityEvent onWaitTimeOver;

    public void StartWaiting()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        onWaitTimeOver.Invoke();
    }
}
