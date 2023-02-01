using UnityEngine;
using System.Collections;

public class AStar
{
    #region List fields

    public static PriorityQueue closedList, openList;

    #endregion

    /// <summary>
    /// Calculate the final path in the path finding
    /// </summary>
    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse(); // 8. Sort the open list in ascending order, ordered by the total cost to reach the target node.
        return list;
    }

    /// <summary>
    /// Calculate the estimated Heuristic cost to the goal
    /// </summary>
    private static float HeuristicEstimateCost(Node curNode, Node goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    /// <summary>
    /// Find the path between start node and goal node using AStar Algorithm
    /// </summary>
    public static ArrayList FindPath(Node start, Node goal)
    {
        //Start Finding the path
        openList = new PriorityQueue();
        openList.Push(start); // 1. First, we start with the starting node and put it in the open list
        start.nodeTotalCost = 0.0f;
        start.estimatedCost = HeuristicEstimateCost(start, goal);

        closedList = new PriorityQueue();
        Node node = null;

        while (openList.Length != 0) // 2. As long as the open list has some nodes in it, we'll perform the following process.
        {
            node = openList.First();

            if (node.position == goal.position)
            {
                return CalculatePath(node);
            }
			
            ArrayList neighbours = new ArrayList();
            GridManager.instance.GetNeighbours(node, neighbours);

            #region CheckNeighbours

            // 4. Get the neighboring nodes of this current node, which are not obstacle types, such as a wall or canyon that can't be passed through.
            for (int i = 0; i < neighbours.Count; i++)
            {
                //Cost between neighbour nodes
                Node neighbourNode = (Node)neighbours[i];

                if (!closedList.Contains(neighbourNode)) // 5. For each neighbor node, check if this neighbor node is already in the closed list. If not we'll calculate the total cost ( F ) for this neighbor node using the following formula:
                {					
					//Cost from current node to this neighbour node
	                float cost = HeuristicEstimateCost(node, neighbourNode); // 3. Pick the node with the least cost F from the open list and keep it as the current node	

                    //Total Cost So Far from start to this neighbour node
                    float totalCost = node.nodeTotalCost + cost;
					
					//Estimated cost for neighbour node to the goal
	                float neighbourNodeEstCost = HeuristicEstimateCost(neighbourNode, goal);

                    //Assign neighbour node properties

                    // 6. Store that cost data in the neighbor node object. Also, store the current node as the parent node as well. Later we'll use this parent node data to trace back the actual path
                    neighbourNode.nodeTotalCost = totalCost;
	                neighbourNode.parent = node;
	                neighbourNode.estimatedCost = totalCost + neighbourNodeEstCost;
	
	                //Add the neighbour node to the list if not already existed in the list
	                if (!openList.Contains(neighbourNode))
	                {
	                    openList.Push(neighbourNode); // 7. Put this neighbor node in the open list.
                    }
                }
            }
			
            #endregion
            
            closedList.Push(node);
            openList.Remove(node); // 9. If there's no more neighbor nodes to process, put the current node in the closed list and remove it from the open list
        }

        //If finished looping and cannot find the goal then return null
        if (node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }

        //Calculate the path based on the final node
        return CalculatePath(node); // 10. Go back to step 2.
    }
}
