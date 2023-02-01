using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image healthImage;


    // Update is called once per frame
    void LateUpdate()
    {
        //this.transform.LookAt(Camera.main.transform);
    }

    public void SetHealth(int newHealth)
    {
        float percent = newHealth / 100.00f;
        this.healthImage.fillAmount = percent;
    }
}
