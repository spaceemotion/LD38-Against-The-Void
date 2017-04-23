using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour {
    public string Scene;


    public void SwitchToScene () {
        SceneManager.LoadScene (Scene, LoadSceneMode.Single);
    }

}
