using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public TestCode tankCode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray.origin, ray.direction, out hitInfo))
            {
                Vector3 targetPos = hitInfo.point;
                this.transform.position = targetPos;
                tankCode.resetTank();
            }
        }
    }
}
