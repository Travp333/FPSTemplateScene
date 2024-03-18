//Author: Sandeep
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{

  public DeathMenu deathScreen;

  public void Death()
  {
    //pauses time so player cannot move
    Time.timeScale = 0;
    //activates the death menu
    deathScreen.gameObject.SetActive(true);
    //makes the cursor visible and unlocks it
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
  }
}
