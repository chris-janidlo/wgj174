using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    public class Paddle : MonoBehaviour
    {
        public float Acceleration = 120;
        public float RangeOfMotion = 1;

        [Range(0, 90)]
        public float DeflectionAngle = 45;

        [Tooltip("Determines how much a ball bounces away from the center as a function of the distance from the center. On the y axis, 0 means 'bounce straight up' and 1 means 'bounce at angle `deflction` away from the center of the paddle.' On the x axis, 0 is the center of the paddle, -1 is the far left edge, and 1 is the far right edge.")]
        public AnimationCurve BallDeflectionByNormalizedDistanceFromCenter = new AnimationCurve
        (
            new Keyframe(-1, 1),
            new Keyframe(0, 0),
            new Keyframe(1, 1)
        );

        public Rigidbody2D Rigidbody;
        public BoxCollider2D Collider;
        public FloatVariable PaddleAxisInput;

        [SerializeField]
        Vector3 _anchor;
        public Vector3 Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;
                transform.position = value;
            }
        }

        void Start ()
        {
            Anchor = transform.position;
        }

        void Update ()
        {
            if (PaddleAxisInput.Value == 0)
            {
                Rigidbody.velocity = Vector2.zero;
            }
            else if (
                !((transform.position == Anchor + Vector3.right * RangeOfMotion && PaddleAxisInput.Value > 0) ||
                (transform.position == Anchor + Vector3.left * RangeOfMotion && PaddleAxisInput.Value < 0))
            ) {
                Rigidbody.velocity += Vector2.right * PaddleAxisInput.Value * Acceleration * Time.deltaTime;
            }

            transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, Anchor.x - RangeOfMotion, Anchor.x + RangeOfMotion),
                transform.position.y,
                transform.position.z
            );
        }

        // assumes paddle is an axis-aligned box
        public Vector2 GetDeflectionVector (float collisionXInWorldSpace)
        {
            var bounds = Collider.bounds;

            float max = bounds.max.x, min = bounds.min.x;
            float normalizedCollisionX = 2 * (collisionXInWorldSpace - min) / (max - min) - 1;

            float deflection = BallDeflectionByNormalizedDistanceFromCenter.Evaluate(normalizedCollisionX);

            Vector2 leftVector = deflectionVectorByAngle(-DeflectionAngle);
            Vector2 centerVector = deflectionVectorByAngle(0);
            Vector2 rightVector = deflectionVectorByAngle(DeflectionAngle);

            if (normalizedCollisionX < 0)
            {
                return Vector2.Lerp(leftVector, centerVector, -deflection);
            }
            else
            {
                return Vector2.Lerp(centerVector, rightVector, deflection);
            }
        }

        Vector2 deflectionVectorByAngle (float angle)
        {
            var rad = -angle * Mathf.Deg2Rad;
            return new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
        }
    }
}
