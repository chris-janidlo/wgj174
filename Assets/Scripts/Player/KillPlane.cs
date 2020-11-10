using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    public class KillPlane : MonoBehaviour
    {
        public VoidEvent BallShouldDie;

        public void OnBallPositionDidChange (Vector2 ballPosition)
        {
            if (ballPosition.y < transform.position.y)
            {
                BallShouldDie.Raise();
            }
        }
    }
}
