using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroSelector : MonoBehaviour
{
    // private TextMeshProUGUI nameText;
    public string heroName;
    [TextArea]
    public string description;
    public bool invited = false;
    public GameObject hero;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectHero() {
      SelectorManager.instance.SelectHero(this);
    }

    void OnValidate() {
      GetComponentInChildren<TextMeshProUGUI>().text = heroName;
    }

}
