using UnityEngine;
using System.Collections;

public class ChaseState : FSMState
{
    public ChaseState(RandomWanderPoints wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Chasing;

        curRotSpeed = 2.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPoint();
    }

    public override void Reason(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        //Set the target position as the player position
        destPos = player.position;

        if (NPC.health <= 30 && !NPC.healing)
        {
            Debug.Log("Switch to Heal State");
            NPC.healing = true;
            NPC.SetTransition(Transition.LowHealth);
        }

        //Check the distance with player tank
        //When the distance is near, transition to attack state
        float dist = Vector3.Distance(npc.position, destPos);
        if (dist <= 30.0f && NPC.seen)
        {
            FSMStateID newState = NPC.gm.camoAttack[Random.Range(1, 100)];
            if (newState == FSMStateID.Cloak)
            {
                Debug.Log("Switch to Cloak state");
                NPC.SetTransition(Transition.goingDark);
            }
            else if (newState == FSMStateID.Attacking)
            {
                Debug.Log("Switch to Attack state");
                NPC.SetTransition(Transition.ReachPlayer);
                NPC.doneDance = false;
            }

        }
        //Go back to patrol is it become too far
        else if (dist >= 40.0f)
        {
            Debug.Log("Switch to Patrol state");
            NPC.SetTransition(Transition.LostPlayer);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();
        if (!NPC.seen)
            return;

        //Rotate to the target point
        destPos = player.position;

        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //Go Forward
        //npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
        //npc.transform.position = Vector3.MoveTowards(npc.position, destPos, Time.deltaTime * curSpeed);
        NPC.agent.SetDestination(destPos);
    }
}
