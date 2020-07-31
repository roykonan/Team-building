using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashMaterial : MonoBehaviour
{
    private Color startColor;
    private MeshRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
      rend = GetComponent<MeshRenderer>();
      startColor = rend.material.color;
    }
    
    public virtual void Flash(Color color, float time) {
      rend.material.color = color;
      Invoke("ResetColor", time);
    }

    public void ResetColor() {
      rend.material.color = startColor;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
