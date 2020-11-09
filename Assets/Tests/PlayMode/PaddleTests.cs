using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityAtoms.BaseAtoms;
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

            paddleAxisInput.Value = 0;
        }

        [TearDown]
        public void TearDown ()
        {
            paddleAxisInput.Value = 0;

            foreach (var paddle in Object.FindObjectsOfType<Paddle>())
            {
                Object.Destroy(paddle);
            }
        }

        static readonly float[] rangeOfMotionValues = new float[] { 1, 2, 2.5f, 13.7f };
        static readonly int[] inputDirectionValues = new int[] { -1, 1 };

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Paddle_ShouldStayWithinRangeOfMotion_WhenMoving ([ValueSource("rangeOfMotionValues")] float rangeOfMotion, [ValueSource("inputDirectionValues")] int inputDirection)
        {
            var paddle = createPaddle();

            paddle.RangeOfMotion = rangeOfMotion;

            float targetXPosition = paddle.OriginalPosition.x + inputDirection * rangeOfMotion;

            while (paddle.transform.position.x != targetXPosition)
            {
                paddleAxisInput.Value = inputDirection;
                yield return null;
            }

            int additionalFramesToTest = 10;

            while (additionalFramesToTest > 0)
            {
                paddleAxisInput.Value = inputDirection;
                Assert.AreEqual(targetXPosition, paddle.transform.position.x);

                additionalFramesToTest--;
                yield return null;
            }
        }

        static readonly int[] numPaddlesValues = new int[] { 1, 2, 3, 5, 10, 20 };

        [UnityTest]
        public IEnumerator AllPaddles_ShouldMove_WhenInputIsGiven ([ValueSource("numPaddlesValues")] int numPaddles, [ValueSource("inputDirectionValues")] int inputDirection)
        {
            var paddles = new List<Paddle>();

            for (int i = 0; i < numPaddles; i++)
            {
                paddles.Add(createPaddle());
            }

            paddleAxisInput.Value = inputDirection;

            yield return null;

            bool inputIsPositive = inputDirection > 0;

            Assert.That
            (
                paddles.All(p =>
                {
                    return inputIsPositive
                        ? p.transform.position.x > 0
                        : p.transform.position.x < 0;
                ),
                $"every paddle moved {(inputIsPositive ? "right" : "left")}
            );
        }

        Paddle createPaddle ()
        {
            return Object.Instantiate(paddlePrefab);
        }
    }
}
