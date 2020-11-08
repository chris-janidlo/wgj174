using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Drepanoid.Tests.PlayMode
{
    public class BallTests
    {
        Ball ball;

        [SetUp]
        public void SetUp ()
        {
            ball = Object.Instantiate(Resources.Load<Ball>("Prefabs/Base Ball"));
        }

        [TearDown]
        public void TearDown ()
        {
            Object.Destroy(ball);
        }

        [UnityTest]
        public IEnumerator Ball_HasGravity ()
        {
            var originalYPos = ball.transform.position.y;
            yield return new WaitForSeconds(1);
            Assert.Less(ball.transform.position.y, originalYPos, "ball should be falling");
        }
    }
}
