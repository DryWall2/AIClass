using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObs : MonoBehaviour
{
    Vector3 left, right, start;

    public float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        left = start;
        right = start;

        left.z -= 300;
        right.z += 300;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(left, right, Mathf.PingPong(Time.time * speed, 1.0f));
    }
}
