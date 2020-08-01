using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    public Waypoints waypoints;
    public GameObject currentWaypoint;
    public float waypointProximity;
    public float heroProximity;
    public float heroProximityForTransition;
    [Tooltip("0-1; 0: slowest hero, 1: fastest hero")]
    public float speedFactor;
    public bool requireAllHeroes = true;
    private List<Hero> activeHeroes;
    private float speed;
    private float minSpeed;
    private float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        speed = 4;
        currentWaypoint = waypoints.GetNextWaypoint();
    }

    public void SetHeroes(List<Hero> heroes) {
      activeHeroes = heroes;
      for(int i =0;i<heroes.Count; i++) {
        Hero hero = heroes[i];
        if(i==0) {
          minSpeed = hero.moveSpeed;
          maxSpeed = hero.moveSpeed;
        }
        else {
          if(hero.moveSpeed<minSpeed) {
            minSpeed = hero.moveSpeed;
          }
          if(hero.moveSpeed>maxSpeed) {
            maxSpeed = hero.moveSpeed;
          }
        }
      }
      speed = minSpeed * (1-speedFactor) + maxSpeed * (speedFactor);
    }

    void OnValidate() {
      speed = minSpeed * (1-speedFactor) + maxSpeed * (speedFactor);
    }
    private void Update2() {
      for(int i = 0;i < activeHeroes.Count;i++) {
        GameObject hero = activeHeroes[i].gameObject;
        Vector3 heroDistance = hero.transform.position - transform.position;
        if(heroDistance.sqrMagnitude>heroProximityForTransition*heroProximityForTransition) {
          return;
        }
      }
      currentWaypoint = waypoints.GetNextWaypoint();
      transform.position = currentWaypoint.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
      // Update2();return;
        //If all living heroes are in the proximity,
        //move forward
        //speed = highest speed value of heroes

        //test vars:
        if(requireAllHeroes) {
          for(int i = 0;i < activeHeroes.Count;i++) {
            GameObject hero = activeHeroes[i].gameObject;
            if(hero.GetComponent<Hero>().IsDead())continue;
            Vector3 heroDistance = hero.transform.position - transform.position;
            if(heroDistance.sqrMagnitude>heroProximity*heroProximity) {
              return;
            }
          }
        }

        Vector3 difference = currentWaypoint.transform.position - transform.position;
        float distance = difference.magnitude;
        if(distance <= waypointProximity) {
          for(int i = 0;i < activeHeroes.Count;i++) {
            GameObject hero = activeHeroes[i].gameObject;
            if(hero.GetComponent<Hero>().IsDead())continue;
            Vector3 heroDistance = hero.transform.position - transform.position;
            if(heroDistance.sqrMagnitude>heroProximityForTransition*heroProximityForTransition) {
              return;
            }
          }
          currentWaypoint = waypoints.GetNextWaypoint();
        } else {
          Vector3 velocity = difference/distance * speed;
          transform.position += velocity * Time.deltaTime;
          transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
