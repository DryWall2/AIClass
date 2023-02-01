using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReanimateState : FSMState
{
    private bool reachedDestination = false;
    public float reanimteTime = 5f;
    private NPCTankController fallenNPC;

    public ReanimateState()
    {
        stateID = FSMStateID.Reanimate;
        //destPos = npc.allyBonePile.transform.position;
        //fallenNPC = npc.allyBonePile.transform.parent.gameObject.GetComponent<NPCTankController>();

        curRotSpeed = 2.0f;
        curSpeed = 100.0f;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if(NPC.allyBonePile == null)
            NPC.SetTransition(Transition.FullHealth);

        destPos = NPC.allyBonePile.transform.position;
        fallenNPC = NPC.allyBonePile.transform.parent.gameObject.GetComponent<NPCTankController>();

        if(fallenNPC == null)
        {
            NPC.SetTransition(Transition.FullHealth);
        }

        if (reanimteTime <= 0)
        {
            Debug.Log("Done Reanimateing");
            reanimteTime = 5f;
            Debug.Log("Switch to Patrol State");
            fallenNPC.reviveSkeleton();
            NPC.SetTransition(Transition.FullHealth);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if (Vector3.Distance(npc.position, destPos) >= 10f) //while the npc hasnt reached the destonation move towards it
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
        else if (!reachedDestination) //as the NPC reaches the destination, make it dance than mark it as reached.
        {
            NPC.anim.Play("dance");
            reachedDestination = true;
        }
        else if (reachedDestination)
        {
            reanimteTime -= Time.fixedDeltaTime;
            Debug.Log("Reanimate Time: " + (int)reanimteTime);
        }
    }
}
