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
      public bool animates = true;
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
    [Range(-1,1)]
    public float knockBackResistence = 0;
    private Rigidbody rb;
    public GameObject attackSpawnLocation;
    public Attack[] attackList;

    public GameObject healthDisplay;
    public GameObject deadDisplay;
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
      if(health<=0) {
        DeadUpdate();
        velocity = Vector3.Lerp(velocity, Vector3.zero, velocityLerp);
        rb.velocity = velocity;
        return;
      }
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
      } else if(isHero&&GameManager.instance.reviveNeeded>0&&prioritizeRevive) {
        FindAndRevive();
      } else if(targetEnemy) {
        TargetEnemy();
        moving = velocity != Vector3.zero;
      } else if(isHero&&GameManager.instance.reviveNeeded>0) {
        FindAndRevive();
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

    public float reviveDistance = 3;
    public float reviveSpeed = 1;
    public bool prioritizeRevive = false;
    private void FindAndRevive() {
      float distance = -1;
      Hero target = null;
      Vector3 targetDiff = Vector3.zero;
      float targetMag = 1;
      foreach(Hero friend in GameManager.instance.heroes) {
        if(friend.health<=0) {
          Vector3 diff = friend.transform.position - transform.position;
          float mag = diff.magnitude;
          if(mag < distance || distance==-1) {
            target = friend;
            distance= mag;
            targetDiff = diff;
            targetMag = mag;
          }
        }
      }
      if(target!=null) {
        transform.rotation = Quaternion.LookRotation(targetDiff);
        if(targetMag<reviveDistance) {
          velocity = Vector3.Lerp(velocity, Vector3.zero, velocityLerp);
          moving = false;
          target.Revive(reviveSpeed);
        } else {
          targetDiff = targetDiff/targetMag * moveSpeed;
          moving = true;
          velocity = Vector3.Lerp(velocity, targetDiff, velocityLerp);
        }
      }
    }

    private float reviveTimer;
    private float reviveTime = 1;
    private bool reviving = false;
    public GameObject reviveDisplay;
    public void Revive(float speed) {
      if(!dead)return;
      reviving = true;
      reviveTimer += Time.deltaTime*speed;
      if(reviveTimer>=reviveTime) {
        health = maxHealth;
        GameManager.instance.reviveNeeded -= 1;
        dead = false;
        deadDisplay.SetActive(false);
        reviveTimer = 0;
        reviveDisplay.SetActive(false);
      }
    }

    private void DeadUpdate() {
      if(!reviving) {
        reviveTimer *= 0.8f;
        reviveDisplay.SetActive(false);
      }
      if(reviving) {
        reviveDisplay.SetActive(true);
        reviveDisplay.transform.localScale = new Vector3(reviveTimer/reviveTime, 1,1);
      }
      reviving = false;
      Quaternion targetRotation =  Quaternion.Euler(-90,0,0);
      model.transform.localRotation = Quaternion.Slerp(model.transform.localRotation, targetRotation, 0.2f);
    }

    [Header("Animation")]
    public float animationFrq;
    public float animationDistance;
    public float animationRotation;
    public float attackHeight;
    public float attackAngle;
    private float animationRamp = 0;
    private float moveAnimationTime = 0;
    private void AnimationUpdate() {
      Vector3 pos = model.transform.localPosition;
      if(moving) {
        if(animationRamp<1) {
          animationRamp += 0.01f;
        }
        moveAnimationTime += Time.deltaTime;
        float targetY = (Mathf.Cos(moveAnimationTime*animationFrq)+1)/2 * animationDistance;
        float rot = Mathf.Cos(moveAnimationTime*animationFrq/2) * animationRotation;
        pos.y = targetY*animationRamp;
        model.transform.localRotation = Quaternion.Slerp(
          model.transform.localRotation,
          Quaternion.Euler(0,0,rot),
          animationRamp);
      } else {
        animationRamp = 0;
        moveAnimationTime = 0;
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
          if(attack.stopWhenInRange&&i==0) {
            stop = true;
          }
          if(attack.animates&&i==0) {
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
          moving = false;
      } else {
        difference = difference/distance;
        difference *= moveSpeed;
        velocity = Vector3.Lerp(velocity, difference, velocityLerp);
        moving = true;
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
    private bool dead;
    public void Damage(float amount) {
      if(dead) {
        health = 0; return;
      }
      health -= amount;
      if(health>maxHealth)health = maxHealth;
      if(health<=0) {
        if(!dead) {
          Die();
          reviveTimer = 0;
          health = 0;
        }
        dead = true;
      }
      UpdateHealthDisplay();
      if(amount>0&&flash) {
        flash.Flash(Color.red, 0.1f);
      }
    }
    
    public void Push(float amount, float time, Vector3 from) {
      Vector3 difference = transform.position - from;
      difference.Normalize();
      difference *= amount * (1-knockBackResistence);
      velocity = difference;
      pushTimer = time;
    }

    public void Pull(float amount, float time, Vector3 target) {
      Vector3 difference = target - transform.position;
      difference.Normalize();
      difference *= amount * (1-knockBackResistence);
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

    public bool IsDead() {
      return health<=0;
    }
}
