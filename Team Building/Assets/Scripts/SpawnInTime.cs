using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInTime : MonoBehaviour
{
    public GameObject toSpawn;
    public float time;
    public bool attachToParent;
    public bool deleteAfterTime;
    public float timeToDelete;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Spawn", time);
    }

    void Spawn() {
      GameObject instance = Instantiate(toSpawn, transform.position, transform.rotation);
      if(attachToParent) {
        instance.transform.parent = transform;
      }
      if(deleteAfterTime) {
        Destroy(instance, timeToDelete);
      }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
