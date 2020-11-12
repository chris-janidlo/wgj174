using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.TestTools;

namespace Drepanoid.Tests.PlayMode
{
    public class BallLifecycleTests
    {
        BallShooter shooter;
        KillPlane killPlane;

        Vector2Variable ballPosition;
        bool ballDidSpawn, ballShouldDie, ballWillDie, ballDidDie;

        [SetUp]
        public void SetUp ()
        {
            shooter = Object.Instantiate(Resources.Load<BallShooter>("Prefabs/Base Ball Shooter"));

            killPlane = Object.Instantiate(Resources.Load<KillPlane>("Prefabs/Kill Plane"));
            killPlane.transform.position = Vector2.down * 2;

            ballPosition = Resources.Load<Vector2Variable>("Atoms/Ball Position");
            ballPosition.Value = Vector2.zero;

            ballDidSpawn = false;
            Resources.Load<VoidEvent>("Atoms/Ball Did Spawn").Register(() => ballDidSpawn = true);

            ballShouldDie = false;
            Resources.Load<VoidEvent>("Atoms/Ball Should Die").Register(() => ballShouldDie = true);

            ballWillDie = false;
            Resources.Load<VoidEvent>("Atoms/Ball Will Die").Register(() => ballWillDie = true);

            ballDidDie = false;
            Resources.Load<VoidEvent>("Atoms/Ball Did Die").Register(() => ballDidDie = true);
        }

        [TearDown]
        public void TearDown ()
        {
            Object.Destroy(shooter.gameObject);
            Object.Destroy(killPlane.gameObject);

            // in case any were missed:
            foreach (var ball in Object.FindObjectsOfType<Ball>())
            {
                Object.Destroy(ball.gameObject);
            }

            ballDidSpawn = false;
            ballWillDie = false;
            ballDidDie = false;
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Shooter_ShouldSpawnBall_Eventually ()
        {
            yield return new WaitUntil(() => ballDidSpawn);
            Assert.True(ballDidSpawn, "ball never spawned");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldDie_AfterShouldDieIsRaised ()
        {
            yield return new WaitUntil(() => ballShouldDie);
            yield return null;

            Assert.True(ballWillDie, "ball should be at least on track to dying, if not already dead");

            yield return new WaitUntil(() => ballDidDie);
            Assert.True(ballDidDie, "ball never died");
        }

        [UnityTest]
        [Timeout(15000)]
        public IEnumerator Ball_ShouldRespawn_AfterDying ()
        {
            yield return new WaitUntil(() => ballDidDie);
            Assert.True(ballDidDie, "ball never died");

            yield return new WaitUntil(() => ballDidSpawn);
            Assert.True(ballDidSpawn, "ball never spawned");
        }
    }
}
