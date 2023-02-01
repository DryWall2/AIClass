using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinUpgrade : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerTankController>().spinTime = true;
            Destroy(this.gameObject);
        }
    }
}
