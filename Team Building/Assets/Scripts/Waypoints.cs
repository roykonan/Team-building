using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public bool loopInsteadOfBackwards;
    private List<GameObject> waypoints;
    private bool backwards;
    public int index = 0;
    // Start is called before the first frame update
    void Awake()
    {
        backwards = false;
        waypoints = new List<GameObject>();
        foreach (Transform child in transform)
        {
            waypoints.Add(child.gameObject);
        }
    }

    public GameObject GetNextWaypoint() {
      GameObject waypoint = waypoints[index];
      if(backwards) {
        index -= 1;
        if(index<0) {
          index = 1;
          backwards = false;
        }
      } else {
        index += 1;
        if(index>=waypoints.Count) {
          if(loopInsteadOfBackwards) {
            index = 0;
          } else {
            index-=2;
            backwards = true;
          }
        }
      }
      return waypoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
