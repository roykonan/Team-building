using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTrigger : MonoBehaviour
{
    private Character hero;
    private void Awake() {
      hero = GetComponentInParent<Character>();
    }
    private void OnTriggerStay(Collider other) {
      if(hero.CanTargetEnemy()) {
        hero.SetTargetEnemy(other.gameObject);
      }
    }
}
