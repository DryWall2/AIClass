using UnityEngine;
using System.Collections;

public class Touch : Sense
{
    void OnTriggerEnter(Collider other)
    {
        Aspect aspect = other.GetComponent<Aspect>();
        if (aspect != null)
        {
            //Check the aspect
            if (aspect.aspectName == aspectName)
            {
                Debug.Log("Enemy Touch Detected");

                this.transform.parent.GetComponent<NPCTankController>().seen = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        this.transform.parent.GetComponent<NPCTankController>().seen = false;
    }


}
