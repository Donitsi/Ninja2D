using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayTheGame : MonoBehaviour {

    public void StartGame()
    {
        StartCoroutine(levelChange());
    }

    IEnumerator levelChange()
    {
        yield return SceneManager.LoadSceneAsync("Level01");
    }
}
