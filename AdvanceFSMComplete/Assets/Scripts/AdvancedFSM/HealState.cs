using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealState : FSMState
{
    private bool isIdle = false;
    private bool fa = false;
    public HealState(Transform restZone)
    {
        destPos = restZone.position;
        stateID = FSMStateID.Heal;

        curRotSpeed = 2.0f;
        curSpeed = 100.0f;

    }

    public override void Reason(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if(NPC.health >= 100)
        {
            NPC.health = 100;
            NPC.SetTransition(Transition.FullHealth);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();
        destPos = GameObject.FindGameObjectWithTag("RestZone").GetComponent<Transform>().position;

        NPC.agent.SetDestination(destPos);

        //1. Find another random patrol point if the current point is reached
        if (Vector3.Distance(npc.position, destPos) <= 10f)
        {
            NPC.health++;
        }
    }
}
