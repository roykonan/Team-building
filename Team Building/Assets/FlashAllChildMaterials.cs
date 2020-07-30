using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashAllChildMaterials : FlashMaterial
{
    private List<Color> startColor = new List<Color>();
    private MeshRenderer[] renderers;
    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer r in renderers) {
          startColor.Add(r.material.color);
        }
    }

    public override void Flash(Color color, float time) {
      foreach(MeshRenderer r in renderers) {
        r.material.color = color;
      }
      Invoke("ResetColors", time);
    }

    void ResetColors() {
      int i = 0;
      foreach(MeshRenderer r in renderers) {
        r.material.color = startColor[i++];
      }
    }
}
