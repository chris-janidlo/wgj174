using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    public class Ball : MonoBehaviour
    {
        public VoidEvent BallDidCollide;
        public Rigidbody2D Rigidbody;
    }
}
