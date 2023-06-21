using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public const string headTag = "Head";
    public const string leftHandTag = "LeftHand";
    public const string rightHandTag = "RightHand";
    public static GameObject Player { get; private set; } = null;
    public static GameObject Head { get; private set; } = null;
    public static GameObject LeftHand { get; private set; } = null;
    public static GameObject RightHand { get; private set; } = null;

    private void Awake()
    {
        EnableCorrespondingPlayer();
    }

    private void Start()
    {
        StartCoroutine(SearchForDevices());
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
            Debug.Log($"Attempting to find tracked GameObjects. Found: \nHead = {(Head == null ? "null" : Head.name)} \nLeft Hand = {(LeftHand == null ? "null" : LeftHand.name)} \nRight Hand = {(RightHand == null ? "null" : RightHand.name)}");
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
