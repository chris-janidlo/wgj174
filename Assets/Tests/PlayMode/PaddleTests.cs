using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Drepanoid.Tests.PlayMode
{
    public class PaddleTests
    {
        FloatVariable paddleAxisInput;
        Paddle paddlePrefab;

        [SetUp]
        public void SetUp ()
        {
            paddleAxisInput = Resources.Load<FloatVariable>("Atoms/Paddle Axis Input");
            paddlePrefab = Resources.Load<Paddle>("Prefabs/Base Paddle");

            Assert.NotZero(paddlePrefab.Acceleration, "tests assume that paddle prefab has some acceleration value");
            Assert.NotZero(paddlePrefab.RangeOfMotion, "tests assume that paddle prefab has some range of motion");

            paddleAxisInput.Value = 0;
        }

        [TearDown]
        public void TearDown ()
        {
            paddleAxisInput.Value = 0;

            foreach (var paddle in Object.FindObjectsOfType<Paddle>())
            {
                Object.Destroy(paddle.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator Paddle_ShouldNotMove_WhenNoInputIsGiven ()
        {
            var paddle = createPaddle();

            paddleAxisInput.Value = 0;

            int additionalFramesToTest = 10;

            while (additionalFramesToTest > 0)
            {
                Assert.Zero(paddle.Rigidbody.velocity.magnitude, $"paddle has a non-zero velocity with {additionalFramesToTest} frames remaining");
                additionalFramesToTest--;
                yield return null;
            }
        }

        static readonly int[] inputDirectionValues = new int[] { -1, 1 };

        [UnityTest]
        //[Timeout(15000)]
        public IEnumerator Paddle_ShouldInstantlyChangeVelocityDirection_WhenInputChanges ([ValueSource("inputDirectionValues")] int initialInputdirection)
        {
            var paddle = createPaddle();

            paddleAxisInput.Value = initialInputdirection;
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            paddleAxisInput.Value = -initialInputdirection;

            yield return new WaitForEndOfFrame();

            Assert.That(Mathf.Sign(paddle.Rigidbody.velocity.x), Is.Not.EqualTo(Mathf.Sign(initialInputdirection)), "the direction of the paddle's velocity should have changed by now");
        }

        static readonly float[] rangeOfMotionValues = new float[] { 1, 2, 2.5f, 13.7f };
        static readonly float[] accelerationValues = new float[] { 120, 100000 };

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Paddle_ShouldStayWithinRangeOfMotion_WhenMoving (
            [ValueSource("rangeOfMotionValues")] float rangeOfMotion,
            [ValueSource("accelerationValues")] float acceleration,
            [ValueSource("inputDirectionValues")] int inputDirection
        ) {
            var paddle = createPaddle();

            paddle.RangeOfMotion = rangeOfMotion;
            paddle.Acceleration = acceleration;

            float targetXPosition = paddle.Anchor.x + inputDirection * rangeOfMotion;

            paddleAxisInput.Value = inputDirection;

            yield return new WaitUntil(() => paddle.transform.position.x == targetXPosition);

            int additionalFramesToTest = 10;

            while (additionalFramesToTest > 0)
            {
                Assert.AreEqual(targetXPosition, paddle.transform.position.x, $"paddle is not at edge of range with {additionalFramesToTest} frames remaining");
                additionalFramesToTest--;
                yield return null;
            }
        }

        static readonly int[] numPaddlesValues = new int[] { 1, 2, 3, 5, 10, 20 };

        [UnityTest]
        public IEnumerator AllPaddles_ShouldMove_WhenInputIsGiven (
            [ValueSource("numPaddlesValues")] int numPaddles,
            [ValueSource("inputDirectionValues")] int inputDirection
        ) {
            var paddles = new List<Paddle>();

            for (int i = 0; i < numPaddles; i++)
            {
                var p = createPaddle();
                p.transform.position = Vector3.up * (i + 1) * 2;
                paddles.Add(p);
            }

            paddleAxisInput.Value = inputDirection;

            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            for (int i = 0; i < numPaddles; i++)
            {
                Paddle p = paddles[i];
                float deltaMovement = p.transform.position.x - p.Anchor.x;

                if (inputDirection > 0)
                {
                    Assert.Positive(deltaMovement, $"paddle {i} should move right");
                }
                else
                {
                    Assert.Negative(deltaMovement, $"paddle {i} should move left");
                }
            }
        }

        Paddle createPaddle ()
        {
            return Object.Instantiate(paddlePrefab);
        }
    }
}
