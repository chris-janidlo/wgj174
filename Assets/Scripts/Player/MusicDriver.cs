using crass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Drepanoid
{
    public class MusicDriver : Singleton<MusicDriver>
    {
        [Header("Ball")]
        public VoidEvent BallDidCollide;
        public AnimationCurve BallFalloff;
        public AudioSource BallSource;

        [Header("Paddle")]
        public FloatVariable PaddleAxisInput;
        public AnimationCurve PaddleFalloff;
        public AudioSource PaddleSource;

        float ballTimer, paddleTimer;

        void Awake ()
        {
            SingletonSetPersistantInstance(this);
        }

        void Start ()
        {
            ballTimer = BallFalloff.keys[BallFalloff.keys.Length - 1].time;
            paddleTimer = PaddleFalloff.keys[PaddleFalloff.keys.Length - 1].time;

            BallDidCollide.Register(() => ballTimer = 0);
        }

        void Update ()
        {
            updateBallAudio();
            updatePaddleAudio();
        }

        void updateBallAudio ()
        {
            ballTimer += Time.deltaTime;
            BallSource.volume = BallFalloff.Evaluate(ballTimer);
        }

        void updatePaddleAudio ()
        {
            if (PaddleAxisInput.Value != 0)
            {
                paddleTimer = 0;
            }
            else
            {
                paddleTimer += Time.deltaTime;
            }

            PaddleSource.volume = PaddleFalloff.Evaluate(paddleTimer);
        }
    }
}
