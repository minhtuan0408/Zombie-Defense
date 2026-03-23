using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AScroller.Demo
{
    public class NextScene : MonoBehaviour
    {
        public void ChangeScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}
