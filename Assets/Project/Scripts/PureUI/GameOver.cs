using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
   public void Continue() => SceneManager.LoadScene("ProtoTutorial");

   public void Restart()
   {
      SceneData.ShouldRespawnInBoss = false;
      Continue();
   }

   public void Quit() => Application.Quit();
}
