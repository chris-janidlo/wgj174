using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    public class Ball : MonoBehaviour
    {
        public float MaxSpeed;
        public float GravityAcceleration;

        public Rigidbody2D Rigidbody;

        public Vector2Variable BallPosition;
        public VoidEvent BallDidCollide, BallDidSpawn, BallWillDie, BallDidDie;

        Vector2 cachedVelocity;

        int shouldDie;

        void Start ()
        {
            BallDidSpawn.Raise();
        }

        void Update ()
        {
            BallPosition.Value= transform.position;
        }

        void FixedUpdate ()
        {
            Rigidbody.velocity += Vector2.down * GravityAcceleration * Time.deltaTime;
            Rigidbody.velocity = Vector2.ClampMagnitude(Rigidbody.velocity, MaxSpeed);

            cachedVelocity = Rigidbody.velocity;
        }

        void OnCollisionEnter2D (Collision2D collision)
        {
            BallDidCollide.Raise();

            var contact = collision.GetContact(0);
            var potentialPaddle = collision.gameObject.GetComponent<Paddle>();

            if (contact.normal == Vector2.up && potentialPaddle != null)
            {
                Rigidbody.velocity = potentialPaddle.GetDeflectionVector(contact.point.x) * potentialPaddle.BounceSpeed;
            }
            else
            {
                Rigidbody.velocity = Vector2.Reflect(cachedVelocity, contact.normal);
            }
        }

        public void OnBallShouldDie ()
        {
            BallWillDie.Raise();

            // this is where you'd play any animations

            Destroy(gameObject);
            BallDidDie.Raise();
        }
    }
}
