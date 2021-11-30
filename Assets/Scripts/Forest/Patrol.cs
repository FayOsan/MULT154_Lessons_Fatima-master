using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public List<GameObject> waypoints;
    private NavMeshAgent agent;
    private const float WP_THRESHOLD = 5.0f;
    private GameObject currentWP;
    private int currntWPIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentWP = GetNextWaypoint();
    }

    GameObject GetNextWaypoint()
    {
        currntWPIndex++;
        if (currntWPIndex == waypoints.Count)
        {
            currntWPIndex = 0;
        }
        Debug.Log("GettingNextWayPoint " + currntWPIndex );
        return waypoints[currntWPIndex];
    }

    // Update is called once per frame
    public void PatrolWaypoints()
    {
        Debug.Log("PatrolWayPoints");
        if(Vector3.Distance(transform.position, currentWP.transform.position) < WP_THRESHOLD)
        {
            currentWP = GetNextWaypoint();
            agent.SetDestination(currentWP.transform.position);
        }

    }
}
