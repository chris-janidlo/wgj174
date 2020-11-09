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
            Object.Destroy(ball.gameObject);
            Object.Destroy(paddle.gameObject);
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceUp_AfterCollidingWithPaddle ()
        {
            yield return genericCollisionTest
            (
                Vector3.down * 10,
                () => ball.Rigidbody.velocity.y > 0,
                "ball y velocity should be positive"
            );
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceRight_AfterCollidingWithRightSideOfPaddle ()
        {
            yield return genericCollisionTest
            (
                new Vector3(-paddle.Collider.bounds.extents.x / 2, -10),
                () => ball.Rigidbody.velocity.x > 0,
                "ball x velocity should be positive"
            );
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceLeft_AfterCollidingWithLeftSideOfPaddle ()
        {
            yield return genericCollisionTest
            (
                new Vector3(paddle.Collider.bounds.extents.x / 2, -10),
                () => ball.Rigidbody.velocity.x < 0,
                "ball x velocity should be negative"
            );
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldRetainSpeed_WhenColliding ()
        {
            yield return new WaitForSeconds(1); // let ball fall for a bit

            ball.GravityAcceleration = 0;
            var preCollisionSpeed = ball.Rigidbody.velocity.magnitude;

            setPaddlePosition(ball.transform.position + Vector3.down * preCollisionSpeed); // position paddle so that the ball will hit it in one second

            yield return new WaitUntil(() => ballCollided);
            yield return new WaitForSeconds(.1f);

            Assert.AreEqual(preCollisionSpeed, ball.Rigidbody.velocity.magnitude, "ball should not lose speed in collisions");
        }

        IEnumerator genericCollisionTest (Vector3 paddleOffsetFromBall, System.Func<bool> assertion, string message)
        {
            setPaddlePosition(paddleOffsetFromBall);

            yield return new WaitUntil(() => ballCollided);
            yield return new WaitForEndOfFrame();

            Assert.That(assertion, message);
        }

        void setPaddlePosition (Vector3 offsetFromBall)
        {
            paddle.Anchor = ball.transform.position + offsetFromBall;
        }
    }
}
