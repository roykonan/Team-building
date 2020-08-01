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
    [Header("Piercing")]
    public bool destroyOnHit = false;
    public int multiShot = 1;
    public GameObject parent;
    public bool hitsEnemy = false;
    public bool hitsHero = false;
    public GameObject parentCharacter;
    public int parentCharacterId;
    public bool canHitSelf = false;
    private Vector3 spawnPosition;
    public GameObject spawnOnHit;
    public float spawnDespawnTime = 1;
    public bool attachSpawnedOnHitToParent = true;
    // Start is called before the first frame update
    void Start()
    {
      if(parentCharacter!=null)
        spawnPosition = parentCharacter.transform.position;
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
        character.Push(pushAmount, pushPullTime, spawnPosition);
      }
      if(pullAmount!=0) {
        character.Pull(pullAmount, pushPullTime, spawnPosition);
      }
      if(stun!=0) {
        character.Stun(stun);
      }
      if(buffAttackSpeed!=0) {
        character.BuffAttackSpeed(buffAttackSpeed, buffTimer);
      }
      if(destroyOnHit) {
        if(--multiShot<=0) {
          Destroy(parent);
        }
      }
      if(spawnOnHit != null) {
        GameObject instance = Instantiate(spawnOnHit, other.transform.position, other.transform.rotation);
        if(spawnDespawnTime!=0)Destroy(instance, spawnDespawnTime);
        if(attachSpawnedOnHitToParent) {
          instance.transform.parent = other.transform;
        }
      }
    }
}
