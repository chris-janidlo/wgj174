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
        public VoidEvent BallDidCollide;

        Vector2 cachedVelocity;

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
    }
}
