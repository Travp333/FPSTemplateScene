using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Created by Andrew Rodriguez with the help of https://www.youtube.com/watch?v=_nRzoTzeyxU&ab_channel=Brackeys
//This script lives on the npc objects
public class NpcDialogue : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject dialogueBox;

    public void Begin(){
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public void OnTriggerEnter(Collider other) //if the player enters the collider of the npc then the dialogue system will trigger
    {
        if (other.tag == "Player")
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
    }

}
