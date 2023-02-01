using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using System;


public class NPCTankController : AdvancedFSM 
{
    #region Variables
    [Header("Firing Rates")]
    public int totalChance;
    public int health;

    [Header("Prefabs and Refernces")]
    public GameObject Bullet;
    public GameObject npcBody;
    //public GameObject healthObject;
    public GameObject fastNPCEffect;
    public GameObject cloakingNPCEffect;
    //public HealthDisplay healthDisplay;
    //public GameObject spinUpgrade;

    //public Image healthBar;
    
    // We overwrite the deprecated built-in `rigidbody` variable.
    new private Rigidbody rigidbody;
    public Transform restZone;
    private Transform home;
    [HideInInspector] public GameManager gm;

    [HideInInspector] public bool canBeHit = true;
    [HideInInspector] public bool doneDance = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool healing = false;
    [HideInInspector] public bool isFast;
    [HideInInspector] public bool[] fastAttackChance = new bool[100];
    // DA OLD
    //private MeshRenderer headMesh;
    
    [HideInInspector] public SkinnedMeshRenderer npcMesh;
    [HideInInspector] public Animation anim;
    [HideInInspector] public MeshRenderer bulletMesh;
    public Transform bulletSpawnPoint2;

    public NavMeshAgent agent;

    //---Haylie stuff---
    public GameObject bonePilePrefab;
    public GameObject allyBonePile; //bonepile seen by this NPC
    private GameObject thisBonePile;//bonepile dropped by this NPC
    //------------------

    public bool forest = false;

    public GameObject touchDetection;
    public Perspective sight;

    public Vector3 noisePos;
    public bool heardNoise;

    public bool seen;

    public bool canMove = false;

    public ParticleSystem.MainModule settings;

    #endregion

    private Action<EventParam> someListener2;

    private void Awake()
    {
        someListener2 = new Action<EventParam>(noiseHerd);
    }

    private void OnEnable()
    {
        EventManagerDelPara.StartListening("boom", noiseHerd);
    }

    private void OnDisable()
    {
        EventManagerDelPara.StopListening("boom", noiseHerd);
    }

    void noiseHerd(EventParam eventParam)
    {
        if (CurrentStateID != FSMStateID.Patrolling)
            return;

        noisePos = eventParam.postition;
        Debug.Log("Heard a noise at: " + eventParam.direction);

        heardNoise = true;
    }


    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        settings = cloakingNPCEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;

        //noisePos = Vector3.zero;

        if (isFast)
        {
            touchDetection.SetActive(false);
            sight.enabled = true;
        }
        else if (!isFast)
        {
            touchDetection.SetActive(true);
            sight.enabled = false;
        }


        #region Setting Variables

        //Get Bullet Mesh
        bulletMesh = Bullet.GetComponent<MeshRenderer>();
        bulletMesh.sharedMaterial.SetColor("_TintColor", Color.yellow);

        //Get the Animation and Mesh components from the npcBody.
        anim = npcBody.GetComponent<Animation>();
        npcMesh = npcBody.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>();

        //Get the target enemy(Player)
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //Get the rest zone
        restZone = GameObject.FindGameObjectWithTag("RestZone").transform;

        //Get the home area
        //home = GameObject.FindGameObjectWithTag("Home").transform;

        //Get the game manager
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        //Get the rigidbody
        rigidbody = GetComponent<Rigidbody>();

        //Get the bullet spawn point
        bulletSpawnPoint2 = gameObject.transform.GetChild(0).transform;

        agent = GetComponent<NavMeshAgent>();
        #endregion

        gm.TanksAlive += 1;
        gm.TanksOn += 1;

        health = 100;

        elapsedTime = 0.0f;

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

        if (isFast)
            fastNPCEffect.SetActive(true);

        //Start Doing the Finite State Machine
        ConstructFSM();
    }

    //Update each frame
    protected override void FSMUpdate()
    {
        //Check for health
        elapsedTime += Time.deltaTime;
    }
    
    protected override void FSMFixedUpdate()
    {
        if (canMove == false)
            return;

        if(seen)
            GameManager.gm.detectedImg.SetActive(true);
        else
            GameManager.gm.detectedImg.SetActive(false);

        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);

        switch (CurrentState)
        {
            case ChaseState chase:
                anim.Play("run");

                cloakingNPCEffect.SetActive(true);
                settings.loop = true;
                settings.startColor = new ParticleSystem.MinMaxGradient(Color.yellow);

                break;
            case DanceState dance:

                cloakingNPCEffect.SetActive(false);

                anim.Play("dance");
                npcMesh.material.SetColor("_Color", Color.white);
                break;
            case OffState offState:

                cloakingNPCEffect.SetActive(false);

                anim.Play("run");
                npcMesh.material.SetColor("_Color", Color.black);
                canBeHit = false;
                break;
            case HealState heal:

                cloakingNPCEffect.SetActive(false);

                anim.Play("run");
                npcMesh.material.SetColor("_Color", Color.blue);
                break;
            case PatrolState patrol:

                cloakingNPCEffect.SetActive(false);

                anim.Play("run");
                npcMesh.material.SetColor("_Color", new Color(0.6343284f, 0.6343284f, 0.6343284f));
                break;
            case AttackState attack:
                anim.Play("attack");

                cloakingNPCEffect.SetActive(true);
                settings.loop = true;
                settings.startColor = new ParticleSystem.MinMaxGradient(Color.red);
                
                break;
            case DeadState dead:
                npcMesh.material.SetColor("_Color", Color.black);
                break;
            case GoToNoiseState noise:

                cloakingNPCEffect.SetActive(false);

                npcMesh.material.SetColor("_Color", Color.gray);
                break;
            //---Haylie stuff---
            case ReanimateState reanimate:

                cloakingNPCEffect.SetActive(false);

                npcMesh.material.SetColor("_Color", Color.magenta);
                break;
            //------------------
        }
    }

    public void SetTransition(Transition t) 
    { 
        PerformTransition(t); 
    }

    private void ConstructFSM()
    {
        //Get the list of points
        /*pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        Transform[] waypoints = new Transform[pointList.Length];
        int i = 0;
        foreach(GameObject obj in pointList)
        {
            waypoints[i] = obj.transform;
            i++;
        }*/

        // DA
        RandomWanderPoints waypoints = null;

        if (forest)
            waypoints = gm.forestSpawnArea.GetComponent<RandomWanderPoints>();
        else
            waypoints = gm.RuinsSpawnArea.GetComponent<RandomWanderPoints>();        

        PatrolState patrol = new PatrolState(waypoints);
        //patrol.AddTransition(Transition.timeToGoHome, FSMStateID.OffDuty);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        //patrol.AddTransition(Transition.timeToDance, FSMStateID.Dance);
        patrol.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        patrol.AddTransition(Transition.Noise, FSMStateID.Noise);
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        //---Haylie stuff---
        patrol.AddTransition(Transition.SawBonePile, FSMStateID.Reanimate);
        //------------------

        ChaseState chase = new ChaseState(waypoints);
        chase.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        chase.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        chase.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        chase.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        chase.AddTransition(Transition.Noise, FSMStateID.Noise);
        chase.AddTransition(Transition.goingDark, FSMStateID.Cloak);
        //---Haylie stuff---
        chase.AddTransition(Transition.SawBonePile, FSMStateID.Reanimate);
        //------------------

        AttackState attack = new AttackState(waypoints);
        attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        attack.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        attack.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        //---Haylie stuff---
        attack.AddTransition(Transition.SawBonePile, FSMStateID.Reanimate);
        //------------------

        DeadState dead = new DeadState();
        dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        //---Haylie stuff---
        dead.AddTransition(Transition.Revive, FSMStateID.Patrolling);
        //------------------

        /*DanceState dance = new DanceState();
        dance.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        dance.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        dance.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        dance.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        OffState offDuty = new OffState(home);
        offDuty.AddTransition(Transition.doneAtHome, FSMStateID.Patrolling);
        offDuty.AddTransition(Transition.NoHealth, FSMStateID.Dead);*/

        HealState heal = new HealState(restZone);
        heal.AddTransition(Transition.FullHealth, FSMStateID.Patrolling);
        heal.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        heal.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        heal.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        CloakState cloakState = new CloakState(waypoints);
        cloakState.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        cloakState.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        cloakState.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        GoToNoiseState noise = new GoToNoiseState(noisePos);
        noise.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        noise.AddTransition(Transition.SawPlayer, FSMStateID.Attacking);
        //noise.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        noise.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        noise.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //---Haylie stuff---
        ReanimateState reanimate = new ReanimateState();
        reanimate.AddTransition(Transition.FullHealth, FSMStateID.Patrolling);
        reanimate.AddTransition(Transition.LowHealth, FSMStateID.Heal);
        reanimate.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        //------------------

        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(dead);
        //AddFSMState(dance);
        //AddFSMState(offDuty);
        AddFSMState(heal);
        AddFSMState(cloakState);
        AddFSMState(noise);
        //---Haylie stuff---
        AddFSMState(reanimate);
        //------------------
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "PBullet")
        {
            Debug.Log("Skeleton was hit!");
            if (canBeHit)
            {
                health -= 10;
                //healthDisplay.SetHealth(health);
            }
            
            if (health <= 0)
            {
                Debug.Log("Switch to Dead State");
                SetTransition(Transition.NoHealth);
                Explode();
            }
            /*else if (health <= 30 && !healing)
            {
                Debug.Log("Switch to Heal State");
                healing = true;
                SetTransition(Transition.LowHealth);
            }*/
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Set Immunity and healing.

        /*if (other.gameObject.tag == "RestZone")
        {
            canBeHit = false;
            StartCoroutine("healWhileResting");
        }
        //---Haylie stuff---
        else*/
        if (other.gameObject.tag == "BonePile")
        {
            switch (CurrentState)
            {
                case PatrolState patrol:
                    allyBonePile = other.gameObject;
                    SetTransition(Transition.SawBonePile);
                    break;
                case AttackState attack:
                    if (UnityEngine.Random.Range(0, gm.TanksAlive) > (gm.TanksAlive / 2))
                    {
                        allyBonePile = other.gameObject;
                        SetTransition(Transition.SawBonePile);
                    }
                    break;
                case ChaseState chase:
                    if (UnityEngine.Random.Range(0, gm.TanksAlive) > (gm.TanksAlive / 2))
                    {
                        allyBonePile = other.gameObject;
                        SetTransition(Transition.SawBonePile);
                    }
                    break;
            }
        }
        //------------------

    }

    private void OnTriggerExit(Collider other)
    {
        //remove immunity and healing.
        if (other.gameObject.tag == "RestZone")
        {
            canBeHit = true;
        }
    }

    protected void Explode()
    {
        if (!isDead)
        {
            isDead = true;
            //---Haylie stuff---
            thisBonePile = Instantiate(bonePilePrefab, this.transform);
            //------------------

            //if (UnityEngine.Random.Range(0, 20) == 1) { Instantiate(spinUpgrade, transform.position, Quaternion.identity); }

            gm.TanksAlive -= 1;
            gm.TanksOn -= 1;
            gm.UpdateObjectiveText();
            gm.setAllAttackChanceArrays(); 

            /*if (gm.TanksAlive == 0)
                gm.EnableEndOfGame(true);*/

            rigidbody.constraints = RigidbodyConstraints.None;

            /*float rndX = UnityEngine.Random.Range(10.0f, 30.0f);
            float rndZ = UnityEngine.Random.Range(10.0f, 30.0f);
            anim.Play("die");
            for (int i = 0; i < 3; i++)
            {
                rigidbody.AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
                rigidbody.velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
            }


            

            Destroy(gameObject, 1.5f);*/
            npcBody.SetActive(false);
        }
    }

    //---Haylie stuff---
    public void reviveSkeleton()
    {
        isDead = false;

        Destroy(thisBonePile);
        npcBody.SetActive(true);
        SetTransition(Transition.Revive);
    }
    //------------------

    /// <summary>
    /// Shoot the bullet from the turret
    /// </summary>
    public void ShootBullet()
    {
        if (fastAttackChance[UnityEngine.Random.Range(0, 100)])
        {
            shootRate = gm.fastBulletRate;
            print("fast bullet!");
        }
        else
        {
            shootRate = gm.normalBulletRate;
        }


        if (elapsedTime >= shootRate)
        {
            
            var temp = Instantiate(Bullet, bulletSpawnPoint2.position, bulletSpawnPoint2.rotation);

            if (isFast) { temp.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red); }
            else { temp.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow); }

            temp.SetActive(true);
            elapsedTime = 0.0f;
            //bulletMesh.sharedMaterial.SetColor("_TintColor", Color.yellow);
        }
    }

    IEnumerator healWhileResting()
    {
        canBeHit = false;

        do
        {
            health += 10;
            //healthDisplay.SetHealth(health);
            yield return new WaitForSeconds(1);

        } while (health < 100);
    }

    public int GetHealth()
    {
        return health;
    }

    public void setFullHealth()
    {
        health = 100;
        //healthDisplay.SetHealth(health);
    }
}
