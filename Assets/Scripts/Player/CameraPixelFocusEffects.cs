using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using crass;

namespace Drepanoid
{
    public class CameraPixelFocusEffects : MonoBehaviour
    {
        public int DefaultPPU;
        public TransitionableFloat CurrentPPU;

        public float ShakeTime, ShakeInterval;
        public int ShakeAmount;
        public AnimationCurve ShakeIntervalCurve, ShakeIntensityCurve;

        public PixelPerfectCamera PixelCamera;

        IEnumerator shakeEnum;

        void Update ()
        {
            PixelCamera.assetsPPU = (int) CurrentPPU.Value;
        }

        public void Shake ()
        {
            if (shakeEnum != null) StopCoroutine(shakeEnum);
            StartCoroutine(shakeEnum = shakeRoutine());
        }

        IEnumerator shakeRoutine ()
        {
            float timer = ShakeTime;

            while (timer > 0)
            {
                var progress = 1 - timer / ShakeTime;

                int offset = Random.Range(1, ShakeAmount) * (RandomExtra.Chance(0.5f) ? -1 : 1);
                CurrentPPU.Value = DefaultPPU + (int) (offset * Mathf.Round(ShakeIntensityCurve.Evaluate(progress)));

                var interval = ShakeInterval * ShakeIntervalCurve.Evaluate(progress);
                timer -= interval;

                if (timer > 0) yield return new WaitForSeconds(interval);
            }

            CurrentPPU.Value = DefaultPPU;
        }
    }
}
