using UnityEngine;
using System.Collections;

public class AttackState : FSMState
{
    public AttackState(RandomWanderPoints wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Attacking;
        curRotSpeed = 2.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPoint();
    }
    //TODO: change distance to something else.
    public override void Reason(Transform player, Transform npc)
    {
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if (NPC.health <= 30 && !NPC.healing)
        {
            Debug.Log("Switch to Heal State");
            NPC.healing = true;
            NPC.SetTransition(Transition.LowHealth);
        }

        //Check the distance with the player tank
        float dist = Vector3.Distance(npc.position, player.position);
        if (dist >= 30.0f && dist < 50.0f)
        {
            //Rotate to the target point
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
            npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            //Go Forward
            //npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
            //npc.transform.position = Vector3.MoveTowards(npc.position, destPos, Time.deltaTime * curSpeed);

            //NPC.agent.destination = destPos;
            NPC.agent.isStopped = false;
            Debug.Log("Switch to Chase State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.SawPlayer);
        }
        //Transition to patrol is the tank become too far
        else if (dist >= 40.0f)
        {
            NPC.agent.isStopped = false;
            Debug.Log("Switch to Patrol State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
        }  
    }

    public override void Act(Transform player, Transform npc)
    {
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();
        //Set the target position as the player position
        //destPos = player.position;
        NPC.agent.isStopped = true;

        //Always Turn the turret towards the player
        Quaternion rotation = Quaternion.LookRotation(player.position - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, rotation, Time.deltaTime * curRotSpeed);

        //Shoot bullet towards the player
        NPC.ShootBullet();
    }
}
