using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateInTime : MonoBehaviour
{
    public GameObject toActivate;
    public float time;
    public bool deactivateAfterTime;
    public float timeToDeactivate;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Activate", time);
    }

    void Activate() {
      toActivate.SetActive(true);
      if(deactivateAfterTime) {
        Invoke("Deactivate", timeToDeactivate);
      }
    }
    void Deactivate() {
      toActivate.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
