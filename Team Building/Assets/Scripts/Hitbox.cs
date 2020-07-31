using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage;
    public float pushAmount;
    public float pullAmount;
    public float pushPullTime = 0.5f;
    public float stun;
    public float buffTimer;
    [Tooltip("Multiplies the speed. (2 is 2x faster)")]
    public float buffAttackSpeed;
    public GameObject parent;
    public bool hitsEnemy = false;
    public bool hitsHero = false;
    public GameObject parentCharacter;
    public int parentCharacterId;
    public bool canHitSelf = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
      Character character = other.gameObject.GetComponent<Character>();
      if(!character)return;
      if(character.isHero && !hitsHero)return;
      if(character.isEnemy && !hitsEnemy)return;
      if(!canHitSelf&&character.id == parentCharacterId)return;
      if(damage!=0) {
        character.Damage(damage);
      }
      if(pushAmount!=0) {
        character.Push(pushAmount, pushPullTime, parentCharacter);
      }
      if(pullAmount!=0) {
        character.Pull(pullAmount, pushPullTime, parentCharacter);
      }
      if(stun!=0) {
        character.Stun(stun);
      }
      if(buffAttackSpeed!=0) {
        character.BuffAttackSpeed(buffAttackSpeed, buffTimer);
      }
    }
}
