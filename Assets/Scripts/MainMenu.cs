using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void ExitGame()
    {
        StartCoroutine(levelChange());
    }

    IEnumerator levelChange()
    {
        yield return SceneManager.LoadSceneAsync("MainMenu");
    }
}
