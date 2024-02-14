using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedDemoHandler : MonoBehaviour
{
    public float endingScreenDelay = 2f;
    private void LoadFinishScene() => SceneManager.LoadScene("Game Completed Screen");
    public void FinishDemo() => Invoke(nameof(LoadFinishScene), endingScreenDelay);
}
