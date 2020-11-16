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
        public int DeathRepeatCount;
        public AnimationCurve DeathRepeatIntervalByCount;

        [Header("Paddle")]
        public FloatVariable PaddleAxisInput;
        public AnimationCurve PaddleFalloff;
        public AudioSource PaddleSource;

        int currentBar => Mathf.FloorToInt(TimingSource.time * BPM / 60 / 4);

        float paddleTimer;
        int ballChordPlayerLooper;

        IEnumerator deathEnum;

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
            playBallChord();
        }

        public void OnDeath ()
        {
            if (deathEnum != null) StopCoroutine(deathEnum);
            StartCoroutine(deathEnum = deathRoutine());
        }

        void playBallChord ()
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

        IEnumerator deathRoutine ()
        {
            for (int i = 0; i < DeathRepeatCount; i++)
            {
                yield return new WaitForSeconds(DeathRepeatIntervalByCount.Evaluate(i));
                playBallChord();
            }
        }
    }
}
