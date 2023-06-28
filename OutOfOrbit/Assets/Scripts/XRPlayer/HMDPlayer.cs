using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMDPlayer : XRPlayer
{
    public override PlayerType type { get => PlayerType.HMD; }
}
