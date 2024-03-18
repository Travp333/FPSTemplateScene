//Author: Brian Meginness
//Debugging: Brian Meginness, Travis Parks
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour
{
    //Menu images
    Image flashlight;
    Image aim;
    Image moveLeft;
    Image moveRight;
    Image moveUp;
    Image moveDown;
    Image jump;
    Image duck;
    Image sprint;
    Image interact;
    Image throwItem;
    Image diveCrouch;
    Image diveJump;
    Image swimUp;
    Image swimDown;
    GameObject errTxt;
    GameObject promptTxt;
    GameObject rebindTxt;

    Controls controls;

    private static IEnumerator err;

    private void Start()
    {
        //Assign components
        controls = GameObject.Find("Data").GetComponentInChildren<Controls>();
        flashlight = GameObject.Find("Flashlight").GetComponent<Image>();
        aim = GameObject.Find("Aim").GetComponent<Image>();
        moveLeft = GameObject.Find("WalkLeft").GetComponent<Image>();
        moveRight = GameObject.Find("WalkRight").GetComponent<Image>();
        moveUp = GameObject.Find("WalkForward").GetComponent<Image>();
        moveDown = GameObject.Find("WalkBack").GetComponent<Image>();
        jump = GameObject.Find("Jump").GetComponent<Image>();
        duck = GameObject.Find("Crouch").GetComponent<Image>();
        sprint = GameObject.Find("Sprint").GetComponent<Image>();
        interact = GameObject.Find("Interact").GetComponent<Image>();
        throwItem = GameObject.Find("Throw").GetComponent<Image>();
        diveCrouch = GameObject.Find("DiveCrouch").GetComponent<Image>();
        diveJump = GameObject.Find("DiveJump").GetComponent<Image>();
        swimUp = GameObject.Find("SwimUp").GetComponent<Image>();
        swimDown = GameObject.Find("SwimDown").GetComponent<Image>();
        errTxt = GameObject.Find("ErrorText");
        promptTxt = GameObject.Find("PromptText");
        rebindTxt = GameObject.Find("RebindText");


        //Update controls
        if (controls)
        {
            draw();
        }
    }

    //Update controls when menu is opened
    private void OnEnable()
    {
        if (controls)
        {
            draw();
        }
    }

    //When a control button is pressed
    public void changeControl(string action)
    {
        StartCoroutine(WaitForKeyPress(action));
    }

    //Coroutine to wait for user input
    private IEnumerator WaitForKeyPress(string action)
    {
        errTxt.SetActive(false);
        try
        {
            StopCoroutine(err);
        }
        catch { }

        StartCoroutine(prompt());

        //Save previous key for the specified action, set to not in use
        KeyCode oldKey = controls.keys[action];
        controls.inUse[oldKey] = false;

        // Wait for key press
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        //Figure out what key was pressed
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {

            if (Input.GetKey(key) && !controls.inUse[key])
            {
                Debug.Log(action + " changed to " + key);
                //Update controls with new key
                controls.keys[action] = key;
                controls.inUse[key] = true;
                draw();
            }
            else if (Input.GetKey(key) && controls.inUse[key])
            {
                err = ErrorMsg();
                StartCoroutine(err);
            }
        }

    }

    private IEnumerator ErrorMsg()
    {
        errTxt.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        errTxt.SetActive(false);
        rebindTxt.SetActive(true);
    }

    private IEnumerator prompt()
    {
        rebindTxt.SetActive(false);
        promptTxt.SetActive(true);
        yield return new WaitUntil(() => Input.anyKey);
        promptTxt.SetActive(false);
    }

    private void draw()
    {
        //Get Keys
        KeyCode flashKey = controls.keys["flashlight"];
        KeyCode aimKey = controls.keys["aim"];
        KeyCode leftKey = controls.keys["walkLeft"];
        KeyCode rightKey = controls.keys["walkRight"];
        KeyCode upKey = controls.keys["walkUp"];
        KeyCode downKey = controls.keys["walkDown"];
        KeyCode jumpKey = controls.keys["jump"];
        KeyCode duckKey = controls.keys["duck"];
        KeyCode runKey = controls.keys["sprint"];
        KeyCode interKey = controls.keys["interact"];
        KeyCode throwKey = controls.keys["throw"];
        KeyCode swimUpKey = controls.keys["swimup"];
        KeyCode swimDownKey = controls.keys["swimdown"];

        errTxt.SetActive(false);
        promptTxt.SetActive(false);
        rebindTxt.SetActive(true);

        //Set images to associated key sprite, or blank if associated sprite doesn't exist
        flashlight.sprite = Resources.Load<Sprite>(flashKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(flashKey.ToString()); 
        swimUp.sprite = Resources.Load<Sprite>(swimUpKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(swimUpKey.ToString()); 
        swimDown.sprite = Resources.Load<Sprite>(swimDownKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(swimDownKey.ToString()); 
        aim.sprite = Resources.Load<Sprite>(aimKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(aimKey.ToString()); 
        moveLeft.sprite = Resources.Load<Sprite>(leftKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(leftKey.ToString());
        moveRight.sprite = Resources.Load<Sprite>(rightKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(rightKey.ToString());
        moveUp.sprite = Resources.Load<Sprite>(upKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(upKey.ToString());
        moveDown.sprite = Resources.Load<Sprite>(downKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(downKey.ToString());
        jump.sprite = Resources.Load<Sprite>(jumpKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(jumpKey.ToString());
        duck.sprite = Resources.Load<Sprite>(duckKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(duckKey.ToString());
        sprint.sprite = Resources.Load<Sprite>(runKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(runKey.ToString());
        interact.sprite = Resources.Load<Sprite>(interKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(interKey.ToString());
        throwItem.sprite = Resources.Load<Sprite>(throwKey.ToString()) == null ? Resources.Load<Sprite>("Blank") : Resources.Load<Sprite>(throwKey.ToString());
        diveCrouch.sprite = duck.sprite;
        diveJump.sprite = jump.sprite;
    }
}
