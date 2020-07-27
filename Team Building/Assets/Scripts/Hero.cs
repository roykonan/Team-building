using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public GameObject pathFollowPoint;
    public float moveSpeed;
    [Tooltip("Miliseconds between attacks")]
    public float attackSpeed;
    public float health;
    private Vector3 velocity = Vector3.zero;
    public float velocityLerp = 0.5f;
    public float followPointProximity = 0.5f;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      Vector3 difference = pathFollowPoint.transform.position - transform.position;
      float distance = difference.magnitude;
      if(distance>followPointProximity) {
        difference = difference/distance;
        difference *= moveSpeed;
        velocity = Vector3.Lerp(velocity, difference, velocityLerp);
      } else if(distance < followPointProximity/2){
        velocity = Vector3.Lerp(velocity, Vector3.zero, velocityLerp);
      }
      // transform.position += velocity * Time.deltaTime;
      rigidbody.velocity = velocity;
    }
}
