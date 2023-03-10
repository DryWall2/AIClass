//Original EventManager script edited to accept Actions (Delegates) with parameters
//from https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManagerDelPara : MonoBehaviour
{

    private Dictionary<string, Action<EventParam>> eventDictionary;

    private static EventManagerDelPara eventManager;

    public static EventManagerDelPara instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManagerDelPara)) as EventManagerDelPara;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventMangerDelPara script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action<EventParam>>();
        }
    }

    public static void StartListening(string eventName, Action<EventParam> listener)
    {
        Action<EventParam> thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        // OR if (instance.eventDictionary.ContainsKey(eventName))
        {
            //Add more event to the existing one
            thisEvent += listener;

            //Update the Dictionary
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            //Add event to the Dictionary for the first time
            thisEvent += listener;
            instance.eventDictionary.Add(eventName, thisEvent);
            Debug.Log("ADDED!");
        }
    }

    public static void StopListening(string eventName, Action<EventParam> listener)
    {
        if (eventManager == null) return;
        Action<EventParam> thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        // OR if (instance.eventDictionary.ContainsKey(eventName))  //use this if 
        {
            //Remove event from the existing one
            thisEvent -= listener;

            //Update the Dictionary
            instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, EventParam eventParam)
    {
        Action<EventParam> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            // OR USE thisEvent.Invoke(eventParam);
            instance.eventDictionary[eventName](eventParam);
        }
    }
}

//Re-usable structure OR class. Add all parameters you need inside it
public struct EventParam
{
    public Vector3 postition;
    public string direction;
}

/*
 public class EventParam: EventArgs
{
    public string param1;
    public int param2;

    public EventParams(string param1, int param2)
    {
        this.param1 = param1;
        this.param2 = param2;
    }
}
*/
