using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundDetection : MonoBehaviour
{
    private PlayerMovement controllerScript;
    float playerVelocity;
    public Material soundDetect;
    public Material normalMaterial;

    public string direction;

    public bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        controllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag != "Player")
            return;

        if ((playerVelocity != 0) && !controllerScript.isCrouching && !done)
        {
            GetComponent<MeshRenderer>().material = soundDetect;

            Debug.Log("Sound Detected!");

            EventParam myparams = default(EventParam);

            myparams.postition = transform.position;

            Debug.Log("DestPos: " + myparams.postition);
            myparams.direction = direction;


            EventManagerDelPara.TriggerEvent("boom", myparams);

            StartCoroutine(wait());
        }
        else
        {
            GetComponent<MeshRenderer>().material = normalMaterial;
        }

        playerVelocity = controllerScript.curSpeed;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag != "Player")
            return;

        GetComponent<MeshRenderer>().material = normalMaterial;
    }

    IEnumerator wait()
    {
        done = true;
        yield return new WaitForSeconds(4f);
        done = false;
    }

}
