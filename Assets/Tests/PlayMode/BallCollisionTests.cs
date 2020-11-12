using NUnit.Framework;
using System.Collections;
using UnityAtoms;
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
            yield return movePaddleAndWaitForCollision(Vector3.down * 10);
            
            Assert.Positive(ball.Rigidbody.velocity.y, "ball y velocity should be positive");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceRight_AfterCollidingWithRightSideOfPaddle ()
        {
            yield return movePaddleAndWaitForCollision(new Vector3(-paddle.Collider.bounds.extents.x / 2, -10));
            
            Assert.Positive(ball.Rigidbody.velocity.x, "ball x velocity should be positive");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceLeft_AfterCollidingWithLeftSideOfPaddle ()
        {
            yield return movePaddleAndWaitForCollision(new Vector3(paddle.Collider.bounds.extents.x / 2, -10));
            
            Assert.Negative(ball.Rigidbody.velocity.x, "ball x velocity should be negative");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator BallSpeed_ShouldBePaddleBounceSpeed_AfterPaddleCollision ()
        {
            yield return movePaddleAndWaitForCollision(Vector3.down * 10);

            Assert.That(ball.Rigidbody.velocity.magnitude, Is.EqualTo(paddle.BounceSpeed).Within(0.1).Percent, "ball speed should be paddle's bounce speed");
        }

        static readonly float[] paddleOffsetValues = new float[] { 0.01f, 0.3f, 0.6f, 1 };

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldBounceAtSameDeflectionAngle_WhenHittingLeftOrRightSide ([ValueSource("paddleOffsetValues")] float paddleOffset)
        {
            float absoluteOffset = paddle.Collider.bounds.extents.x * paddleOffset;

            ball.transform.position = Vector3.zero;
            ball.Rigidbody.velocity = Vector3.zero;

            yield return movePaddleAndWaitForCollision(new Vector3(-absoluteOffset, -10, 0));

            Vector3 leftExitVelocity = ball.Rigidbody.velocity;

            ball.transform.position = Vector3.zero;
            ball.Rigidbody.velocity = Vector3.zero;

            ballCollided = false;
            yield return movePaddleAndWaitForCollision(new Vector3(absoluteOffset, -10, 0));

            Vector3 rightExitVelocity = ball.Rigidbody.velocity;

            Assert.That(leftExitVelocity.y, Is.EqualTo(rightExitVelocity.y).Within(0.01).Percent, "ball should exit from both sides at the same y velocity");
            Assert.That(leftExitVelocity.x, Is.EqualTo(-rightExitVelocity.x).Within(0.01).Percent, "ball x velocity should be mirrored when comparing right and left");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldRetainSpeed_WhenCollidingWithAnythingButAPaddle ()
        {
            yield return new WaitForSeconds(1); // let ball fall for a bit

            ball.BaseGravityAcceleration = 0;
            var preCollisionSpeed = ball.Rigidbody.velocity.magnitude;

            // use wall since paddle introduces bounce speed
            var wall = new GameObject("wall");
            wall.transform.position = ball.transform.position + Vector3.down * preCollisionSpeed; // positioned so that the ball will hit it in ~one second
            var wallCollider = wall.AddComponent<BoxCollider2D>();
            wallCollider.size = new Vector2(10, 2);

            yield return new WaitUntil(() => ballCollided);
            yield return new WaitForSeconds(.1f);

            Assert.AreEqual(preCollisionSpeed, ball.Rigidbody.velocity.magnitude, "ball should not lose speed in non-paddle collisions");

            Object.Destroy(wall.gameObject);
        }

        IEnumerator movePaddleAndWaitForCollision (Vector3 paddleOffsetFromBall)
        {
            paddle.Anchor = ball.transform.position + paddleOffsetFromBall;
            yield return new WaitUntil(() => ballCollided);
        }
    }
}
