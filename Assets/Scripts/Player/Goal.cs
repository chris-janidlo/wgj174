using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Drepanoid
{
    public class Goal : MonoBehaviour
    {
        public UnityEvent BallReachedGoal;

        void OnTriggerEnter2D (Collider2D collision)
        {
            if (collision.GetComponent<Ball>() != null)
            {
                BallReachedGoal.Invoke();
            }
        }
    }
}
