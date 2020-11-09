using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Drepanoid
{
    // translates input from Unity's InputSystem to an atom
    public class InputTranslator : MonoBehaviour
    {
        public FloatVariable PaddleAxisInput;

        public void OnPaddleAxis (InputAction.CallbackContext context)
        {
            PaddleAxisInput.Value = context.ReadValue<float>();
        }
    }
}
