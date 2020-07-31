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
      [HideInInspector]
      public float attackTimer;
      public float projectileSpeed;
      public bool cullBasedOnSpeedAndRange;
      public bool stopWhenInRange;
      public bool Update(GameObject spawnLocation, GameObject target, int id, float timeScale) {
        bool doesAttack = false;
        attackTimer += Time.deltaTime * timeScale;
        if(attackTimer>attackSpeed) {
          doesAttack = true;
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
        return doesAttack;
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

    public GameObject healthDisplay;
    [Header("path following debug")]
    public GameObject pathFollowPoint;
    public float followPointProximity = 0.5f;
    public GameObject targetEnemy;
    public bool moving = false;

    [HideInInspector]
    public bool isHero;
    [HideInInspector]
    public bool isEnemy;

    private float stunTimer;
    private float pushTimer;
    private FlashMaterial flash;
    private GameObject model;

    // Start is called before the first frame update
    void Start()
    {
        id = Character.ID++;
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        flash = GetComponentInChildren<FlashMaterial>();
        model = transform.Find("Model").gameObject;
    }

    // Update is called once per frame
    private void Update() {
      if(attackSpeedBuffTimer>0) {
        attackSpeedBuffTimer -= Time.deltaTime;
        if(attackSpeedBuffTimer <= 0) {
          attackSpeedScale = 1;
        }
      }
      if(pushTimer>0) {
        pushTimer -= Time.deltaTime;
        moving = false;
      }
      if(stunTimer>0) {
        StunUpdate();
        moving = false;
      } else if(pushTimer>0) {
      }
      else if(targetEnemy) {
        TargetEnemy();
        moving = velocity != Vector3.zero;
      } else if(pathFollowPoint) {
        FollowPath();
        moving = velocity != Vector3.zero;
      } else {
        moving = false;
        velocity = Vector3.Lerp(velocity, Vector3.zero, velocityLerp);
      }
      rb.velocity = velocity;
      AnimationUpdate();
    }

    [Header("Animation")]
    public float animationFrq;
    public float animationDistance;
    public float animationRotation;
    public float attackHeight;
    public float attackAngle;
    private float animationRamp = 0;
    private void AnimationUpdate() {
      Vector3 pos = model.transform.localPosition;
      if(moving) {
        if(animationRamp<1) {
          animationRamp += 0.01f;
        }
        float targetY = (Mathf.Cos(Time.time*animationFrq)+1)/2 * animationDistance;
        float rot = Mathf.Cos(Time.time*animationFrq/2) * animationRotation;
        pos.y = targetY*animationRamp;
        model.transform.localRotation = Quaternion.Slerp(
          model.transform.localRotation,
          Quaternion.Euler(0,0,rot),
          animationRamp);
      } else {
        animationRamp = 0;
        float deadLerp = 0.2f;
        pos.y *= (1-deadLerp);
        model.transform.localRotation = Quaternion.Slerp(
          model.transform.localRotation,Quaternion.Euler(0,0,0), deadLerp);
      }
      model.transform.localPosition = pos;
    }

    private void StunUpdate() {
      stunTimer -= Time.deltaTime;
    }

    private float EaseInOutQuad(float t) {
      return t<.5 ? 2*t*t : -1+(4-2*t)*t;
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
          bool doesAttack = attack.Update(attackSpawnLocation, targetEnemy, id, attackSpeedScale);
          // if(doesAttack) model.transform.localPosition += Vector3.up*1f;
          if(attack.stopWhenInRange) {
            stop = true;
            Vector3 pos = model.transform.localPosition;
            float t = attack.attackTimer/attack.attackSpeed;
            t = EaseInOutQuad(t);
            pos.y = t * attackHeight;
            model.transform.localPosition = pos;
            model.transform.localRotation = Quaternion.Euler(
              (1-2*t)*attackAngle,0,0);
          }
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
      if(amount>0&&flash) {
        flash.Flash(Color.red, 0.1f);
      }
    }
    
    public void Push(float amount, float time, GameObject from) {
      Vector3 difference = transform.position - from.transform.position;
      difference.Normalize();
      difference *= amount;
      velocity = difference;
      pushTimer = time;
    }

    public void Pull(float amount, float time, GameObject target) {
      Vector3 difference = target.transform.position - transform.position;
      difference.Normalize();
      difference *= amount;
      velocity = difference;
      pushTimer = time;
    }

    public void Stun(float amount) {
      stunTimer = amount;
      velocity = Vector3.zero;
    }

    private float attackSpeedBuffTimer;
    private float attackSpeedScale = 1;
    public void BuffAttackSpeed(float amount, float time) {
      attackSpeedScale = amount;
      attackSpeedBuffTimer = time;
    }

    public virtual void Die() {

    }
}
