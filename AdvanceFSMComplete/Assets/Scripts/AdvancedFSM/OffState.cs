using UnityEngine;
using System.Collections;

public class OffState : FSMState
{
    public float timeLeft = 10;

    private bool done = false;

    public OffState(Transform homePos)
    {
        stateID = FSMStateID.OffDuty;

        destPos = homePos.position;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
        timeLeft = Random.Range(5, 10);
    }

    public override void Reason(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if (!done)
        {
            NPC.gm.TanksOn -= 1; 
            done = true;
            Debug.Log("Tank has officaly gone off duty.");
        }

        if (timeLeft <= 0)
        {
            NPC.gm.TanksOn += 1;
            done = false;
            goHome = false;
            NPC.canBeHit = true;
            timeLeft = Random.Range(5, 10);

            NPC.SetTransition(Transition.doneAtHome);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        //Rotate to the target point
        /*Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);*/

        NPC.agent.SetDestination(destPos);

        if (Vector3.Distance(npc.position, destPos) <= 130.0f)
        {
            timeLeft -= Time.fixedDeltaTime;
            Debug.Log("Off State time: " + (int)timeLeft);
            if(timeLeft > 0)
            {
                if (NPC.gm.TanksOn < NPC.gm.TanksAlive)
                    timeLeft = -1;
            }
            NPC.setFullHealth();
        }


    }
}