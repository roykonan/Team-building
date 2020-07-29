using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage;
    public float push;
    public float pull;
    public float stun;
    public GameObject parent;
    public bool hitsEnemy = false;
    public bool hitsHero = false;
    public GameObject parentCharacter;
    public int parentCharacterId;
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
      if(character.id == parentCharacterId)return;
      if(damage!=0) {
        character.Damage(damage);
      }
      if(push!=0) {
        character.Push(push, parentCharacter);
      }
      if(pull!=0) {
        character.Pull(pull, parentCharacter);
      }
      if(stun!=0) {
        character.Stun(stun);
      }
    }
}
