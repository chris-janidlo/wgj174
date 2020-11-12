using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    public class KillPlane : MonoBehaviour
    {
        public VoidEvent BallShouldDie;

        void OnCollisionEnter2D (Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Ball>() != null)
            {
                BallShouldDie.Raise();
            }
        }
    }
}
