using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PatrolState : FSMState
{
     
    public PatrolState(RandomWanderPoints wp)
    {

        waypoints = wp;
        stateID = FSMStateID.Patrolling;

        curRotSpeed = 2.0f;
        curSpeed = 100.0f;
    }

    

    public override void Reason(Transform player, Transform npc)
    {

        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if (NPC.health <= 30 && !NPC.healing)
        {
            Debug.Log("Switch to Heal State");
            NPC.healing = true;
            NPC.SetTransition(Transition.LowHealth);
        }

        if (NPC.seen)
            NPC.SetTransition(Transition.SawPlayer);
        if (NPC.heardNoise)
        {
            NPC.SetTransition(Transition.Noise); 
            Debug.Log("HeardNoise If in Patrol");

        }


        /*if (Vector3.Distance(npc.position, player.position) >= 400f && danceTime == true && goHome == false)
        {
            Debug.Log("Dance time");
            NPC.SetTransition(Transition.timeToDance);
            danceTime = false;
        }

        if (Vector3.Distance(npc.position, player.position) >= 400f && goHome == true)
        {
            Debug.Log("Go Home. From Patrol State");
            NPC.SetTransition(Transition.timeToGoHome);
            goHome = false;
        }*/
    }

    public override void Act(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();
        NPC.agent.SetDestination(destPos);

        //1. Find another random patrol point if the current point is reached
        if (Vector3.Distance(npc.position, destPos) <= 10f)
        {
            Debug.Log("Reached the destination point, calculating the next point");
            /*if (UnityEngine.Random.Range(1, 5) == 1 && NPC.heardNoise == false)
            {
                danceTime = true;
            }
            else if (UnityEngine.Random.Range(1, 10) == 1 && npc.GetComponent<NPCTankController>().gm.TanksAlive > 1 && npc.GetComponent<NPCTankController>().gm.TanksOn > 1 && NPC.heardNoise == false)
            {
                goHome = true;
            }*/
            FindNextPoint();
            NPC.agent.SetDestination(destPos);
        }

        //2. Rotate to the target point
        //Quaternion targetRot = Quaternion.LookRotation(destPos - npc.position);
        //npc.rotation = Quaternion.Slerp(npc.rotation, targetRot, Time.deltaTime * curRotSpeed);

        //3. Go Forward
        //npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
        //npc.transform.position = Vector3.MoveTowards(npc.position, destPos, Time.deltaTime * curSpeed);
        //Debug.Log("destPos: [" + destPos + "]");
    }


}