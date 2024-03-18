using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Created by Andrew Rodriguez with the help of https://www.youtube.com/watch?v=_nRzoTzeyxU&ab_channel=Brackeys
//This class is serializable so we can enter the npc name and sentences in the unity editor
//Instances of this class are made and passed to the DialogueManager's functions
[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea(3, 10)]
    public string[] sentences;
}
