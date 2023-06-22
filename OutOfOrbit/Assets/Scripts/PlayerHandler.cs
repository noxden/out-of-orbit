using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-101)]
public class PlayerHandler : MonoBehaviour
{
    private const string headTag = "Head";
    private const string leftHandTag = "LeftHand";
    private const string rightHandTag = "RightHand";
    
    public static PlayerHandler instance { get; private set; }
    public GameObject Player { get; private set; } = null;
    public GameObject Head { get; private set; } = null;
    public GameObject LeftHand { get; private set; } = null;
    public GameObject RightHand { get; private set; } = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(this);

        EnableCorrespondingPlayer();
    }

    private void Start()
    {
        StartCoroutine(SearchForDevices());
    }

    //TODO: The following code should not be here but on a separate script on the CAVEPlayer prefab. 
    //      It also does not account for the Tracked Pose Driver not sending any tracking data, causing the head to spin rapidly whenever that happens.
    private void Update()
    {
        if (Head != null && Settings.playMode == PlayMode.CAVE)
        {
            Vector3 correctionVector = new Vector3(90, 0, 0);
            Head.transform.Rotate(correctionVector, Space.Self);
        }
    }

    private void EnableCorrespondingPlayer()
    {
        GameObject HMDPlayer = GameObject.FindGameObjectWithTag("HMDPlayer");
        GameObject CAVEPlayer = GameObject.FindGameObjectWithTag("CAVEPlayer");

        switch (Settings.playMode)
        {
            case PlayMode.HMD:
                HMDPlayer.SetActive(true);
                CAVEPlayer.SetActive(false);
                Player = HMDPlayer;
                break;
            case PlayMode.CAVE:
                HMDPlayer.SetActive(false);
                CAVEPlayer.SetActive(true);
                Player = CAVEPlayer;
                break;
            default:
                Debug.LogError($"Playmode {Settings.playMode} is neither HMD nor CAVE. Something must have went wrong.", this);
                break;
        }

    }

    private IEnumerator SearchForDevices()
    {
        int maxNumberOfAttempts = 10;
        int waitDurationBetweenAttemptsInSeconds = 1;

        int numberOfAttempts = 0;
        while ((Head == null || LeftHand == null || RightHand == null) && numberOfAttempts < maxNumberOfAttempts)
        {
            // Debug.Log($"Attempting to find tracked GameObjects. Found: \nHead = {(Head == null ? "null" : Head.name)} \nLeft Hand = {(LeftHand == null ? "null" : LeftHand.name)} \nRight Hand = {(RightHand == null ? "null" : RightHand.name)}");
            Head = GameObject.FindGameObjectWithTag(headTag);
            LeftHand = GameObject.FindGameObjectWithTag(leftHandTag);
            RightHand = GameObject.FindGameObjectWithTag(rightHandTag);
            numberOfAttempts += 1;
            yield return new WaitForSeconds(waitDurationBetweenAttemptsInSeconds);
        }
        Debug.Log($"Search concluded after {numberOfAttempts}/{maxNumberOfAttempts} attempts. Found the following GameObjects: \nHead = {(Head == null ? "null" : Head.name)} \nLeft Hand = {(LeftHand == null ? "null" : LeftHand.name)} \nRight Hand = {(RightHand == null ? "null" : RightHand.name)}");
        yield return null;
    }
}
