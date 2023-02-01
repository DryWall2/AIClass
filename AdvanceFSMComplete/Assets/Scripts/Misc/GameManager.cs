using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class GameManager : MonoBehaviour 
{
    //public variables
    public float timerValue = 360;
    public float gameSpeed = 1.0f;
    //public int MinTanks = 4;
    //public int MaxTanks = 8;
    public int TanksAlive;
    public int TanksOn;
    public GameObject GameCanvas;
    public GameObject TanksSpawns;
    public GameObject TankFolder;
    public GameObject TankPrefab;
    //public RandomWanderPoints rndPoints;

    public int baseFastChance = 10;
    public int chanceIncrease = 5;
    public float fastBulletRate = .5f;
    public float normalBulletRate = 1.06f;

    



    //private variables
    public int numTanks;
    public MenuManager menu;
    //private Level levelManager;
    public bool gameOver = false;

    public int artifactsFound;
    //DA

    public static GameManager gm;
    public List<FSMStateID> camoAttack = new List<FSMStateID>();
    public GameObject detectedImg;

    [Header("Final Game Settings")]
    public bool Final;
    public GameObject forestSpawnArea;
    public GameObject RuinsSpawnArea;

    public int minForestSpawn;
    public int maxForestSpawn;
    public int numForestTanks;

    public int minRuinSpawn;
    public int maxRuinSpawn;
    public int numRuinTanks;

    public int fastMax;





    private void Awake()
    {
        
        gm = this;
       /* if(GameObject.FindObjectOfType<Level>() != null)
        {
            levelManager = GameObject.FindObjectOfType<Level>();

            MinTanks = MinTanks + (levelManager.level + 1);
           MaxTanks = MaxTanks + (levelManager.level + 2);
        }
        else
        {
            Debug.LogError("No level holder. Start game from main menu");
        }*/
    }

    // Use this for initialization
    void Start () 
    {
        for (int i = 0; i < 100; i++)
        {
            if (i <= 10)
                camoAttack.Add(FSMStateID.Cloak);
            if (i > 10)
                camoAttack.Add(FSMStateID.Attacking);
        }

        spawnTanks();
        setAllAttackChanceArrays();

        //Set the gravity Settings
        Physics.gravity = new Vector3(0, -500.0f, 0);

        //find all the references needed that im too lazy to set by hand
        menu = GameCanvas.GetComponent<MenuManager>();
        menu.ObjectiveText.text = artifactsFound + " / 7" + " Artifacts.";
    }
	
	// Update is called once per frame
	void Update () 
    {
        Time.timeScale = gameSpeed;

        if (!gameOver)
        {
            if (timerValue > 0)
            {
                timerValue -= Time.deltaTime;
                updateTimerText();

            }
            else if (timerValue <= 0 && TanksAlive > 0)
            {
                EnableEndOfGame(false);
            }

            if (artifactsFound == 7)
            {
                EnableEndOfGame(true);
            }
        }
	}

    public void EnableEndOfGame(bool didPlayerWin)
    {
        gameOver = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (didPlayerWin)
        {
            menu.EndOfGameText.text = "Congrats! You won!";
            //levelManager.level += 1;
        }
        else
        {
            menu.EndOfGameText.text = "Better luck next time.";
            menu.playAgainText.text = "Restart the game";
            //levelManager.level = 0;
        }

        menu.EndOfGamePanel.SetActive(true);
    }

    public void UpdateObjectiveText()
    {
        Debug.Log("updateTimerCalled");
        menu.ObjectiveText.text = artifactsFound + "/ 7" + " Artifacts.";
    }

    private void updateTimerText()
    {
        float minutes = Mathf.FloorToInt(timerValue / 60);
        float seconds = Mathf.FloorToInt(timerValue % 60);

        menu.TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void spawnTanks()
    {

        if (Final)
        {
            numForestTanks = Random.Range(minForestSpawn, (maxForestSpawn + 1));
            numRuinTanks = Random.Range(minRuinSpawn, (maxRuinSpawn + 1));

            numTanks = numForestTanks + numRuinTanks;
            TanksAlive = numForestTanks + numRuinTanks;

            doRandomSpawn(forestSpawnArea, numForestTanks);
            doRandomSpawn(RuinsSpawnArea, numRuinTanks);
        }
        /*else
        {
            numTanks = Random.Range(MinTanks, (MaxTanks + 1));
            //Debug.Log(numTanks);
            //Debug.Log("number of tanks chosen between " + MinTanks + " and " + MaxTanks + ": " + numTanks);

            int j = 0;
            for (int i = 0; i < numTanks; i++)
            {
                if (j >= TanksSpawns.transform.childCount - 1)
                    j = 0;
                else
                    j++;

                GameObject tempTank = Instantiate(TankPrefab, TanksSpawns.transform.GetChild(j).position, Quaternion.identity);
                tempTank.transform.SetParent(TankFolder.transform);

                if (Random.Range(1, GameObject.FindObjectOfType<Level>().fastMax + 1) == 1)
                    tempTank.GetComponent<NPCTankController>().isFast = true;
            }

        

        }*/
    }

    private void doRandomSpawn(GameObject area, int num)
    {
        Bounds spawnArea = area.GetComponent<Renderer>().bounds;
        int i = 0;
        NavMeshHit hit;

        Vector3 point = new Vector3();

        while(i != num)
        {
            float randX = Random.Range(spawnArea.min.x, spawnArea.max.x);
            float randZ = Random.Range(spawnArea.min.z, spawnArea.max.z);

            point = new Vector3(randX, 0, randZ);

            if (NavMesh.SamplePosition(point, out hit, 90.0f, NavMesh.AllAreas))
            {
                point = hit.position;
                var temp = Instantiate(TankPrefab, point, Quaternion.identity);
                temp.GetComponent<NPCTankController>().forest = (area.transform.name == "Forest Wander Points");
                temp.transform.SetParent(TankFolder.transform);

                if (Random.Range(1, fastMax + 1) == 1)
                    temp.GetComponent<NPCTankController>().isFast = true;

                i++;
            }

        }
        
    }

    public void remakeList()
    {
        int cloakingChance = 10 + ((numTanks - TanksAlive) * 5);
        for(int i = 0; i < 100; i++)
        {
            if (i < cloakingChance)
                camoAttack[i] = FSMStateID.Cloak;
            else
                camoAttack[i] = FSMStateID.Attacking;
        }
    }

    public void setAttackChanceArray(NPCTankController npc)
    {
        if (npc != null)
        {
            npc.totalChance = baseFastChance;
            int npcsKilled = numTanks - TanksAlive;

            if (npc.isFast)
                npc.totalChance = npc.totalChance * 3;

            npc.totalChance = npc.totalChance + (npcsKilled * chanceIncrease);
            for (int i = 0; i < 100; i++)
            {
                if (i < npc.totalChance)
                    npc.fastAttackChance[i] = true;
                else
                    npc.fastAttackChance[i] = false;
            }
        }
    }

    public void setAllAttackChanceArrays()
    {
        for (int i = 0; i < TankFolder.transform.childCount;  i++)
        {
            setAttackChanceArray(TankFolder.transform.GetChild(i).gameObject.GetComponent<NPCTankController>());
        }
    }
}
