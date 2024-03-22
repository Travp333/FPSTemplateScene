//Author: Brian Meginness
//Debugging: Brian Meginness
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public Dictionary<string, KeyCode> keys;
    public Dictionary<KeyCode, bool> inUse;

    public static bool exists = false;

    private void Start()
    {
        exists = true;
    }

    private void Awake()
    {
        if (!exists)
        {
            GameObject.DontDestroyOnLoad(this);
            //A dictionary containing game actions and associated keys
            keys = new Dictionary<string, KeyCode>()
            {
                {"walkUp",KeyCode.W},
                {"walkDown",KeyCode.S},
                {"walkLeft",KeyCode.A},
                {"walkRight",KeyCode.D},
                {"jump",KeyCode.Space},
                {"duck",KeyCode.C},
                {"sprint",KeyCode.LeftShift},
                {"interact",KeyCode.E},
                {"throw",KeyCode.Mouse0},
                {"aim",KeyCode.Mouse1},
            };

            //Dictionary for what keys on the keyboard are in use
            inUse = new Dictionary<KeyCode, bool>();

            //FOR all possible keys, set to not in use
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                try
                {
                    inUse.Add(key, false);
                }
                catch
                {

                }

            }

            //FOR each key being used, set in use to true
            foreach (KeyCode key in keys.Values)
            {
                inUse[key] = true;
            }

        }
        else
        {
            Destroy(this);
        }
    }
}
