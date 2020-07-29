﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject heroesHolder;
    public WaypointFollower waypointFollower;
    // Start is called before the first frame update
    void Start()
    {
        List<Hero> heroes = new List<Hero>();
        int i =0;
        foreach (Hero hero in heroesHolder.GetComponentsInChildren<Hero>())
        {
            heroes.Add(hero);
            hero.pathFollowPoint = waypointFollower.transform.GetChild(i++).gameObject;
        }
        waypointFollower.SetHeroes(heroes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
