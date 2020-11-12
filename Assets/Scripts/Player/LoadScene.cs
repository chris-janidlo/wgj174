using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityAtoms.SceneMgmt;

namespace Drepanoid
{
    public class LoadScene : MonoBehaviour
    {
        public SceneField Scene;

        public void Load ()
        {
            SceneManager.LoadScene(Scene);
        }
    }
}
