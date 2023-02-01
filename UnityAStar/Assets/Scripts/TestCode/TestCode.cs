using UnityEngine;
using System.Collections;

public class TestCode : MonoBehaviour 
{
    private Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }

    public ArrayList pathArray;
	
    GameObject objStartCube, objEndCube;
	
    private float elapsedTime = 0.0f;
    public float intervalTime = 1.0f; //Interval time between path finding

    public GameObject tank;
    public int index = 0;
    public Vector3 nextPos;
    public int speed = 5;

	// Use this for initialization
	void Start () 
    {
        objStartCube = GameObject.FindGameObjectWithTag("Start");
        objEndCube = GameObject.FindGameObjectWithTag("End");

        tank.transform.position = objStartCube.transform.position;
        nextPos = tank.transform.position;
        //AStar Calculated Path
        pathArray = new ArrayList();
        FindPath();
	}
	
	// Update is called once per frame
	void Update () 
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            FindPath();
        }

        if(tank.transform.position == nextPos) // Move Tank
        {
            if (index < pathArray.Count)
            {
                Node temp = (Node)pathArray[index];
                nextPos = temp.position;

                tank.transform.position = Vector3.MoveTowards(tank.transform.position, nextPos, speed * Time.deltaTime);
                index += 1;              
               
            }
        }
        else
        {
            Debug.Log("Moving");
            tank.transform.position = Vector3.MoveTowards(tank.transform.position, nextPos, speed * Time.deltaTime);
        }
        
	}

    void FindPath()
    {
        startPos = objStartCube.transform;
        endPos = objEndCube.transform;

        //Assign StartNode and Goal Node
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));

        pathArray = AStar.FindPath(startNode, goalNode);
    }

    public void resetTank()
    {
        Debug.Log("resetTank called");
        index = 0;
        pathArray.Clear();
        objStartCube.transform.position = tank.transform.position;
        nextPos = tank.transform.position;

        //tank.transform.position = objStartCube.transform.position;
        //nextPos = objStartCube.transform.position;
    }

    void OnDrawGizmos()
    {
        if (pathArray == null)
            return;

        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            };
        }
    }
}