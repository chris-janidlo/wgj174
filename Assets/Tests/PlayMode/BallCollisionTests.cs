using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Drepanoid.Tests.PlayMode
{
    public class BallCollisionTests
    {
        Ball ball;
        Paddle paddle;

        bool ballCollided;

        [SetUp]
        public void SetUp ()
        {
            ball = Object.Instantiate(Resources.Load<Ball>("Prefabs/Base Ball"));

            paddle = Object.Instantiate
            (
                Resources.Load<Paddle>("Prefabs/Base Paddle"),
                new Vector3(137, 137, 137), // spawn far away from ball
                Quaternion.identity
            );

            ballCollided = false;
            ball.BallDidCollide.Register(() => ballCollided = true);
        }

        [TearDown]
        public void TearDown ()
        {
            Object.Destroy(ball);
            Object.Destroy(paddle);
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceUp_AfterCollidingWithPaddle ()
        {
            setPaddlePosition(Vector3.down * 10);

            yield return new WaitUntil(() => ballCollided);

            Assert.Positive(ball.Rigidbody.velocity.y, "ball y velocity should be positive");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceRight_AfterCollidingWithRightSideOfPaddle ()
        {
            setPaddlePosition(new Vector3(-1, -10));

            yield return new WaitUntil(() => ballCollided);

            Assert.Positive(ball.Rigidbody.velocity.x, "ball x velocity should be positive");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceLeft_AfterCollidingWithLeftSideOfPaddle ()
        {
            setPaddlePosition(new Vector3(1, -10));

            yield return new WaitUntil(() => ballCollided);

            Assert.Negative(ball.Rigidbody.velocity.x, "ball x velocity should be negative");
        }

        void setPaddlePosition (Vector3 offsetFromBall)
        {
            paddle.transform.position = ball.transform.position + offsetFromBall;
        }
    }
}
