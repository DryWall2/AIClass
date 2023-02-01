using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakState : FSMState
{
    float cloakTime = Random.Range(3.0f, 6.0f);
    bool cloaked = false;


    public CloakState(RandomWanderPoints wp)
    {
        waypoints = wp;
        stateID = FSMStateID.Cloak;

        curRotSpeed = 1.0f;
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

        if (!cloaked)
        {
            NPC.npcMesh.enabled = false;
            if (NPC.isFast)
                NPC.fastNPCEffect.SetActive(false);

            NPC.cloakingNPCEffect.SetActive(true);
            NPC.settings.loop = false;

            NPC.agent.isStopped = true;
            cloaked = true;
            Debug.Log("Cloaked");
        }

        if (cloakTime <= 0)
        {
            NPC.npcMesh.enabled = true;
            if (NPC.isFast)
                NPC.fastNPCEffect.SetActive(true);

            NPC.cloakingNPCEffect.SetActive(false);
            NPC.agent.isStopped = false;
            cloaked = false;
            Debug.Log("un-Cloaked");

            NPC.SetTransition(Transition.LostPlayer);
        }

    }

    public override void Act(Transform player, Transform npc)
    {
        //get a reference to the NPCTank Controller
        NPCTankController NPC = npc.gameObject.GetComponent<NPCTankController>();

        if (cloakTime > 0)
        {
            Time.timeScale = 1f;
            cloakTime -= Time.deltaTime;
            Debug.Log(cloakTime);
        }
    }
}
