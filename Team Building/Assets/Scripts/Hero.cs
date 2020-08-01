using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{
    void Awake()
    {
      isHero = true;
    }
    public override void Die(){
      deadDisplay.SetActive(true);
      GameManager.instance.reviveNeeded += 1;
    }
}
