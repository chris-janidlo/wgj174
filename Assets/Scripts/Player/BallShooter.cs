using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drepanoid
{
    public class BallShooter : MonoBehaviour
    {
        public bool UseBallMaxSpeed;
        public float Speed;

        public Ball BallPrefab;

        IEnumerator Start ()
        {
            yield return new WaitForSeconds(1);
            Shoot();
        }

        public void Shoot ()
        {
            var ball = Instantiate(BallPrefab, transform.position, Quaternion.identity);
            ball.Rigidbody.velocity = transform.up * (UseBallMaxSpeed ? ball.MaxSpeed : Speed);
        }
    }
}
