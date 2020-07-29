using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
  private void Awake() {
    isEnemy = true;
  }
  public override void Die() {
    Destroy(gameObject);
  }
}
