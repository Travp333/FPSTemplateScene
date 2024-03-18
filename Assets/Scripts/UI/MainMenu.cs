//Author: Brian Meginness
//Debugging: Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Menu components
    GameObject mainMenu;
    GameObject settingsMenu;
    GameObject levelsMenu;

    // Start is called before the first frame update
    void Start()
    {
        //Get menu components
        mainMenu = gameObject.transform.Find("Main").gameObject;
        settingsMenu = gameObject.transform.Find("Settings").gameObject;
        levelsMenu = gameObject.transform.Find("LevelSelect").gameObject;

        //Make sure it starts on the main menu
        back();
    }

    //Start a new game
    /*This will probably be expanded if we incorporate 
     * difficulty settings and naming save files */
    public void newGame()
    {
        SceneManager.LoadScene(1);
    }

    //Load a saved game
    /* This is a placeholder, our save system is not
     * finished enough for this to be functional yet */
    public void loadGame()
    {
        Debug.Log("Dummy until save system is up and running");
    }

    //Starts the game from the specified level
    public void levelSelect()
    {
        levelsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    //Quits the game
    public void quitGame()
    {
        Debug.Log("Game quitting from menu");
        Application.Quit();
    }

    //Show settings menu
    public void settings()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    //Returns to main menu
    public void back()
    {
        levelsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

}
