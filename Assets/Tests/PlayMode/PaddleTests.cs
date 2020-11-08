using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Drepanoid.Tests.PlayMode
{
    public class PaddleTests
    {
        Paddle paddle;

        [SetUp]
        public void SetUp ()
        {
            paddle = Object.Instantiate(Resources.Load<Paddle>("Prefabs/Base Paddle"));
        }

        [TearDown]
        public void TearDown ()
        {
            Object.Destroy(paddle);
        }

        [Test]
        public void Paddle_ShouldMove_WhenInputIsGiven ()
        {
            Assert.Fail(); // TODO
        }
    }
}
