using System;
using UnityEngine;

namespace Structs 
{
    [Serializable]
    public struct JumpData
    {
        [field: SerializeField] public float JumpForce;
        [field: SerializeField] public float lastGroundedYPos;
    }
}
