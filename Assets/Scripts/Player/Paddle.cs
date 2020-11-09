using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    public class Paddle : MonoBehaviour
    {
        public FloatVariable PaddleAxisInput;
        public float RangeOfMotion;

        public Vector3 OriginalPosition { get; private set; }
    }
}
