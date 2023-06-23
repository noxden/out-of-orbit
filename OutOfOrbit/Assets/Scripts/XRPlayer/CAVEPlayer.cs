using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAVEPlayer : XRPlayer
{
    public override PlayerType type { get => PlayerType.CAVE; }


    private void LateUpdate()
    {
        if (head != null)    // This does not account for the Tracked Pose Driver not sending any tracking data, causing the head to spin rapidly whenever that happens.
        {
            Vector3 correctionVector = new Vector3(90, 0, 0);
            head.transform.Rotate(correctionVector, Space.Self);
        }
    }
}
