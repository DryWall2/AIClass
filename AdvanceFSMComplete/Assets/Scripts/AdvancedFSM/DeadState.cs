using UnityEngine;
using System.Collections;

public class DeadState : FSMState
{
    bool done = false;
    public DeadState() 
    {
        stateID = FSMStateID.Dead;
    }

    public override void Reason(Transform player, Transform npc)
    {

    }

    public override void Act(Transform player, Transform npc)
    {
        if (!done)
        {
            Debug.Log("Remaking the list");
            npc.GetComponent<NPCTankController>().gm.remakeList();
            done = true;
        }
        
    }
}
