//Author: Sandeep
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public string mainMenuLevel;

    //used for the Restart Button
    public void RestartGame()
    {
      //calls the LoadPlayer method in the PlayerStats class
	    //FindObjectOfType<PlayerStats>().LoadPlayer();
      //de-activates the death menu
      this.gameObject.SetActive(false);
      //unpauses time
      Time.timeScale = 1;
      //locks the cursor 
      Cursor.lockState = CursorLockMode.Locked;
    }

    //used for the Back to Main Menu Button
    public void BackToMainMenu()
    {
        //unpauses time
        Time.timeScale = 1;
        //loads the start menu
        SceneManager.LoadScene(mainMenuLevel);
    }
}
