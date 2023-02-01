using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horseMovement : MonoBehaviour
{
    public Transform[] waypoints = new Transform[5];
    private Transform destination;
    private bool isIdling = false;
    public float idleTime = 5f;
    private float timer;
    public float moveSpeed = 100f;
    public float rotationSpeed = 2f;
    public Animator anim;
    public Rigidbody rb;

    void Start()
    {
        destination = getNewDestination();
        anim.SetTrigger("Moving");
    }

    private void FixedUpdate()
    {
        if (!isIdling)
        {
            Quaternion targetRot = Quaternion.LookRotation(destination.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

            this.transform.position = Vector3.MoveTowards(this.transform.position, destination.position, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(this.gameObject.transform.position, destination.position) <= 100f)
            {
                isIdling = true;
                timer = idleTime;
                anim.SetInteger("Random", Random.Range(0, 17));
                anim.SetTrigger("Idling");
            }
        }
        else
        {
            rb.ResetInertiaTensor();
            timer -= Time.deltaTime;
            anim.SetInteger("Random", Random.Range(0, 17));
            if (timer <= 0)
            {
                destination = getNewDestination();
                isIdling = false;
                anim.SetTrigger("Moving");
            }
        }
    }

    public Transform getNewDestination()
    {
        Transform previousDest;
        bool newDest = false;

        if (destination == null)
            previousDest = null;
        else
            previousDest = destination;

        do
        {
            destination = waypoints[Random.Range(0, waypoints.Length)];

            if (destination != previousDest)
                newDest = true;
        } while (!newDest);

        return waypoints[Random.Range(0, waypoints.Length)];
    }
}
