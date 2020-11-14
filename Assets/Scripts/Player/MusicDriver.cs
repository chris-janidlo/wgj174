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
        public AudioSource TimingSource;
        public int BPM;
        public List<AudioSource> BallChordPlayers;
        public List<AudioClip> ChordToPlayByCurrentBar;

        [Header("Paddle")]
        public FloatVariable PaddleAxisInput;
        public AnimationCurve PaddleFalloff;
        public AudioSource PaddleSource;

        int currentBar => Mathf.FloorToInt(TimingSource.time * BPM / 60 / 4);

        float paddleTimer;
        int ballChordPlayerLooper;

        void Awake ()
        {
            SingletonSetPersistantInstance(this);
        }

        void Start ()
        {
            paddleTimer = PaddleFalloff.keys[PaddleFalloff.keys.Length - 1].time;
        }

        void Update ()
        {
            updatePaddleAudio();
        }

        public void OnBallDidCollide ()
        {
            BallChordPlayers[ballChordPlayerLooper].PlayOneShot(ChordToPlayByCurrentBar[currentBar]);
            ballChordPlayerLooper = (ballChordPlayerLooper + 1) % BallChordPlayers.Count;
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
