using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public GameObject target;
    public float lerp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      Vector3 targetPosition = Vector3.zero;
      int i = 0;
      foreach(Hero hero in GameManager.instance.heroes) {
        targetPosition += hero.transform.position;
        i++;
      }
      if(i!=0) {
        targetPosition *= 1f/i;
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerp);
      }
    }
}
