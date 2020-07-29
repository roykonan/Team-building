using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTrigger : MonoBehaviour
{
    private Hero hero;
    private void Start() {
      hero = GetComponentInParent<Hero>();
    }
    private void OnTriggerStay(Collider other) {
      if(hero.CanTargetEnemy()) {
        hero.SetTargetEnemy(other.gameObject);
      }
    }
}
