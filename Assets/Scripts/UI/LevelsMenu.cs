//Author: Brian Meginness
//Debugging: Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{

    // On level select, load appropriate scene
    public void startLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

}
