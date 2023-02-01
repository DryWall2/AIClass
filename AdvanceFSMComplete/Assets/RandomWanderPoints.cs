using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomWanderPoints : MonoBehaviour
{
    private Bounds wanderArea;

    // Start is called before the first frame update
    void Start()
    {
        wanderArea = this.GetComponent<Renderer>().bounds;
    }

    public Vector3 getNewWanderPoint()
    {
        bool isPointOnNavMesh = false;
        Vector3 newPoint = new Vector3();
        NavMeshHit hit;

        int temp = 0;

        do
        {
            newPoint = choosePoint();

            if (NavMesh.SamplePosition(newPoint, out hit, 90.0f, NavMesh.AllAreas))
            {
                newPoint = hit.position;
                isPointOnNavMesh = true;
            }
            Debug.Log("isPointOnNavMesh -> " + isPointOnNavMesh);

            temp += 1;
        }
        while (!isPointOnNavMesh && temp < 10);
        //Debug.Log("iterations run while looking for point: " + temp);

        return newPoint;
    }

    private Vector3 choosePoint()
    {
        Vector3 point = new Vector3();

        Random.seed = System.DateTime.Now.Millisecond;
        //pick a random point in the area
        float randomX = Random.Range(wanderArea.min.x, wanderArea.max.x);
        float randomZ = Random.Range(wanderArea.min.z, wanderArea.max.z);

        point = new Vector3(randomX, 0, randomZ);

        Debug.Log("point -> " + point);
        return point;
    }


}
