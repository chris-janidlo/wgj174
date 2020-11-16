using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityAtoms.BaseAtoms;

namespace Drepanoid
{
    public class Goal : MonoBehaviour
    {
        public AnimationCurve TimeScaleByDistanceFromCollider;
        public float Delay;

        public Vector2Variable BallPosition;
        public UnityEvent BallReachedGoal;

        public BoxCollider2D Collider;

        IEnumerator victoryEnum;

        void Update ()
        {
            var dist = Vector2.Distance(transform.position, BallPosition.Value);
            Time.timeScale = TimeScaleByDistanceFromCollider.Evaluate(dist);
        }

        void OnTriggerEnter2D (Collider2D collision)
        {
            if (collision.GetComponent<Ball>() != null && victoryEnum == null)
                StartCoroutine(victoryEnum = victoryRoutine());
        }

        IEnumerator victoryRoutine ()
        {
            yield return new WaitForSecondsRealtime(Delay);
            BallReachedGoal.Invoke();

            victoryEnum = null;
        }
    }
}
