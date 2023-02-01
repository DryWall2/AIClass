using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNoiseState : FSMState
{
    public GoToNoiseState(Vector3 noisePos)
    {
        stateID = FSMStateID.Noise;
        destPos = noisePos;

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

        destPos = NPC.noisePos;

        if (NPC.seen)
            NPC.SetTransition(Transition.SawPlayer);

        if (Vector3.Distance(npc.position, destPos) < 9)
        {
            NPC.heardNoise = false;
            NPC.SetTransition(Transition.LostPlayer);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if (Vector3.Distance(npc.position, destPos) >= 10f && NPC.heardNoise) //while the npc hasnt reached the destonation move towards it
        {
            NPC.anim.Play("run");

            //Rotate to the target point
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //Go Forward
            //npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
            //npc.transform.position = Vector3.MoveTowards(npc.position, destPos, Time.deltaTime * curSpeed);
            NPC.agent.SetDestination(destPos);
        }
    }
}
