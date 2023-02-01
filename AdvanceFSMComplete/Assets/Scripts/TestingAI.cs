using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAI : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent navAgent;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        navAgent.destination = player.position;
    }
}
