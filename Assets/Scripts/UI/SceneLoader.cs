using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlashCandy.UI
{
    /// <summary>
    /// This class is responsible for changing the scenes. This has to be eventually moved to a separate system, persistent between scenes.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {

        [SerializeField]
        private int toLoadID;

        public void ChangeScene()
        {
            SceneManager.LoadScene(toLoadID);
        }
    }
}
