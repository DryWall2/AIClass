using UnityEngine;
using System.Collections;

public class DanceState : FSMState
{
    public float timeLeft = 2;
    public bool doneDancing;

    public DanceState()
    {
        stateID = FSMStateID.Dance;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();
        NPC.agent.isStopped = true;

        if (NPC.health <= 30 && !NPC.healing)
        {
            Debug.Log("Switch to Heal State");
            NPC.healing = true;
            NPC.SetTransition(Transition.LowHealth);
        }

        if (timeLeft <= 0)
        {
            Debug.Log("Done dancing");
            doneDancing = true;
            timeLeft = 2;
            Debug.Log("Switch to Chase State");
            NPC.doneDance = true;
            npc.gameObject.GetComponent<NPCTankController>().agent.isStopped = false;
            NPC.SetTransition(Transition.LostPlayer);
        }

    }

    public override void Act(Transform player, Transform npc)
    {
        timeLeft -= Time.fixedDeltaTime;
        Debug.Log("Dance Time: " + (int)timeLeft);
        npc.gameObject.GetComponent<NPCTankController>().agent.isStopped = true;
    }
}