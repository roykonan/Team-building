using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public bool loopInsteadOfBackwards;
    private List<GameObject> waypoints;
    private bool backwards;
    public int index = 0;
    public bool stopAtTheEnd;
    public GameObject youWinPrefab;
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
      if(index>=waypoints.Count&&stopAtTheEnd) {
        GameObject w = waypoints[waypoints.Count-1];
        Instantiate(youWinPrefab, w.transform.position, Quaternion.identity);
        return w;
      }
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
          if(stopAtTheEnd) {
            index = waypoints.Count;
          }
          else if(loopInsteadOfBackwards) {
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
