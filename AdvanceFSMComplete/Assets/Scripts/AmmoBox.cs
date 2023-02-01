using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, .5f, 0 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            int bulletsAdd = 0;
            bulletsAdd = 20 - other.GetComponent<PlayerMovement>().bulletsLeft;
            other.GetComponent<PlayerMovement>().bulletsLeft += bulletsAdd;
            Destroy(this.gameObject);
        }
    }
}
