using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    public class BallShooter : MonoBehaviour
    {
        public bool UseBallMaxSpeed;
        public float Speed;

        public float Delay;

        public Ball BallPrefab;

        void Start ()
        {
            StartCoroutine(shootRoutine());
        }
        
        public void OnBallDidDie ()
        {
            StartCoroutine(shootRoutine());
        }

        IEnumerator shootRoutine ()
        {
            yield return new WaitForSeconds(Delay);

            var ball = Instantiate(BallPrefab, transform.position, Quaternion.identity);
            ball.Rigidbody.velocity = transform.up * (UseBallMaxSpeed ? ball.MaxSpeed : Speed);
        }
    }
}
