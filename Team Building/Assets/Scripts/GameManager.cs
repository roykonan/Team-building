using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject heroesHolder;
    public WaypointFollower waypointFollower;
    public List<Hero> heroes;
    public static GameManager instance;
    public int reviveNeeded = 0;
    // Start is called before the first frame update
    void Start()
    {
      instance = this;
        heroes = new List<Hero>();
        int i =0;
        if(SelectorManager.instance==null) {
          foreach (Hero hero in heroesHolder.GetComponentsInChildren<Hero>())
          {
              heroes.Add(hero);
              hero.pathFollowPoint = waypointFollower.transform.GetChild(i++%waypointFollower.transform.childCount).gameObject;
          }
        } else {
          foreach (HeroSelector hs in SelectorManager.instance.activeHeroes) {
            GameObject instance = Instantiate(hs.hero, heroesHolder.transform.position, heroesHolder.transform.rotation);
            instance.transform.parent = heroesHolder.transform;
            Hero hero = instance.GetComponent<Hero>();
            heroes.Add(hero);
            GameObject waypoint = waypointFollower.transform.GetChild(i++%waypointFollower.transform.childCount).gameObject;
            hero.pathFollowPoint = waypoint;
            hero.transform.position += waypoint.transform.localPosition;
          }
        }
        waypointFollower.SetHeroes(heroes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
