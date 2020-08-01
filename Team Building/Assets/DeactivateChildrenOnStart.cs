using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateChildrenOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
