using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [System.Serializable]
    public class Attack {
      public GameObject attackPrefab;
      [Tooltip("seconds between attacks")]
      public float attackSpeed;
      public float attackRange;
      [Tooltip("seconds to destroy attack. 0 is forever")]
      public float destroyAttackInSeconds;
      public bool attachToParent;
      private float attackTimer;
      public float projectileSpeed;
      public bool cullBasedOnSpeedAndRange;
      public bool stopWhenInRange;
      public void Update(GameObject spawnLocation, GameObject target, int id) {
        attackTimer += Time.deltaTime;
        if(attackTimer>attackSpeed) {
          attackTimer -= attackSpeed;
          GameObject instance = Instantiate(attackPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation);
          foreach(Hitbox hitbox in instance.GetComponentsInChildren<Hitbox>()) {
            hitbox.parentCharacterId = id;
            hitbox.parentCharacter = spawnLocation;
          }
          if(attachToParent) {
            instance.transform.parent = spawnLocation.transform;
          }
          if(destroyAttackInSeconds!=0) {
            Destroy(instance, destroyAttackInSeconds);
          }
          if(projectileSpeed!=0) {
            instance.GetComponent<Rigidbody>().velocity = spawnLocation.transform.forward*projectileSpeed;
            if(cullBasedOnSpeedAndRange) {
              Destroy(instance, attackRange/projectileSpeed);
            }
          }
        }
      }
    }
    public static int ID = 0;
    [HideInInspector]
    public int id;
    [Tooltip("Units per second")]
    public float moveSpeed;
    public float maxHealth;
    private float health;
    private Vector3 velocity = Vector3.zero;
    public float velocityLerp = 0.5f;
    private Rigidbody rb;
    [SerializeField]
    public GameObject attackSpawnLocation;
    public Attack[] attackList;
    private GameObject targetEnemy;

    public GameObject healthDisplay;
    [Header("path following debug")]
    public GameObject pathFollowPoint;
    public float followPointProximity = 0.5f;

    [HideInInspector]
    public bool isHero;
    [HideInInspector]
    public bool isEnemy;

    private float stunTimer;

    // Start is called before the first frame update
    void Start()
    {
        id = Character.ID++;
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    // Update is called once per frame
    private void Update() {
      if(stunTimer>0) {
        StunUpdate();
      } else if(targetEnemy) {
        TargetEnemy();
      } else if(pathFollowPoint) {
        FollowPath();
      }
      rb.velocity = velocity;
    }

    private void StunUpdate() {
      stunTimer -= Time.deltaTime;
      velocity = Vector3.zero;
    }

    private void TargetEnemy() {
      Vector3 difference = targetEnemy.transform.position - transform.position;
      difference.y = 0;
      float distance = difference.magnitude;
      if(distance>0) {
        transform.rotation = Quaternion.LookRotation(difference);
      }
      bool stop = false;
      for(int i =0;i<attackList.Length;i++) {
        Attack attack = attackList[i];
        if(distance<attack.attackRange) {
          attack.Update(attackSpawnLocation, targetEnemy, id);
          if(attack.stopWhenInRange)stop = true;
        }
      }
      if(stop) {
          velocity = Vector3.Lerp(velocity, Vector3.zero, velocityLerp);
      } else {
        difference = difference/distance;
        difference *= moveSpeed;
        velocity = Vector3.Lerp(velocity, difference, velocityLerp);
      }
    }

    private void FollowPath() {
      Vector3 difference = pathFollowPoint.transform.position - transform.position;
      float distance = difference.magnitude;
      if(distance>followPointProximity) {
        difference = difference/distance;
        difference *= moveSpeed;
        velocity = Vector3.Lerp(velocity, difference, velocityLerp);
      } else if(distance < followPointProximity/2){
        velocity = Vector3.Lerp(velocity, Vector3.zero, velocityLerp);
      }
      if(velocity!=Vector3.zero) {
        transform.rotation = Quaternion.LookRotation(velocity);
      }
    }

    public bool CanTargetEnemy() {
      return targetEnemy == null;
    }

    public void SetTargetEnemy(GameObject enemy) {
      targetEnemy = enemy;
    }

    private void UpdateHealthDisplay() {
      if(healthDisplay) {
        healthDisplay.transform.localScale = new Vector3(health/maxHealth, 1,1);
      }
    }

    public void Damage(float amount) {
      health -= amount;
      if(health>maxHealth)health = maxHealth;
      if(health<0)Die();
      UpdateHealthDisplay();
    }
    
    public void Push(float amount, GameObject from) {
      Vector3 difference = transform.position - from.transform.position;
      difference.Normalize();
      difference *= amount;
      velocity = difference;
    }

    public void Pull(float amount, GameObject target) {
      Vector3 difference = target.transform.position - transform.position;
      difference.Normalize();
      difference *= amount;
      velocity = difference;
    }

    public void Stun(float amount) {
      stunTimer = amount;
    }

    public virtual void Die() {

    }
}
