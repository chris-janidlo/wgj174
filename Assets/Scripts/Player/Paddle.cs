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
        public float BounceSpeed = 10;
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
        public LineRenderer RangeOfMotionIndicator;

        [SerializeField]
        Vector3 _anchor;
        public Vector3 Anchor
        {
            get => _anchor;
            set
            {
                RangeOfMotionIndicator.transform.parent = transform;
                RangeOfMotionIndicator.transform.position = Vector3.zero;

                _anchor = value;
                transform.position = value;

                drawRangeOfMotionIndicator();
                RangeOfMotionIndicator.transform.parent = null;
            }
        }

        public Vector3 LeftExtent => Anchor + Vector3.left * RangeOfMotion;
        public Vector3 RightExtent => Anchor + Vector3.right * RangeOfMotion;

        void Start ()
        {
            RangeOfMotionIndicator.transform.parent = null;

            Anchor = transform.position;
        }

        void Update ()
        {
            float input = PaddleAxisInput.Value;

            if (input == 0 || Mathf.Sign(input) != Mathf.Sign(Rigidbody.velocity.x))
            {
                Rigidbody.velocity = Vector2.zero;
            }

            if (
                !((transform.position == RightExtent && input > 0) ||
                (transform.position == LeftExtent && input < 0))
            )
            {
                Rigidbody.velocity += Vector2.right * input * Acceleration * Time.deltaTime;
            }

            transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, Anchor.x - RangeOfMotion, Anchor.x + RangeOfMotion),
                transform.position.y,
                transform.position.z
            );
        }

        void OnDestroy ()
        {
            if (RangeOfMotionIndicator != null)
                Destroy(RangeOfMotionIndicator.gameObject);
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
                return Vector2.Lerp(centerVector, leftVector, deflection);
            }
            else
            {
                return Vector2.Lerp(centerVector, rightVector, deflection);
            }
        }

        void drawRangeOfMotionIndicator ()
        {
            RangeOfMotionIndicator.positionCount = 2;
            RangeOfMotionIndicator.SetPositions(new Vector3[] { LeftExtent, RightExtent });
        }

        Vector2 deflectionVectorByAngle (float angle)
        {
            var rad = -angle * Mathf.Deg2Rad;
            return new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
        }
    }
}
