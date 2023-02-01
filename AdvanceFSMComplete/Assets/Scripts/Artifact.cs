using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{

    private void Update()
    {
        transform.Rotate(0, .5f, 0 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {

            GameManager.gm.artifactsFound += 1;
            Destroy(this.gameObject);
            GameManager.gm.UpdateObjectiveText();
        }
    }

}
