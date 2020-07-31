using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectorManager : MonoBehaviour
{
    public static SelectorManager instance;
    public TextMeshProUGUI informationText;
    public Button inviteButton;
    public int maxHeroes = 4;
    public Color selectedColor;
    public Color defaultColor;
    private int heroesCount = 0;
    private HeroSelector selectedHero;
    private List<HeroSelector> activeHeroes;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SelectHero(HeroSelector heroselector) {
      informationText.text = heroselector.heroName + "\n" + heroselector.description;
      selectedHero = heroselector;
      if(heroselector.invited) {
        inviteButton.GetComponentInChildren<TextMeshProUGUI>().text = "kick";
        inviteButton.interactable = true;
      } else if(heroesCount<maxHeroes){
        inviteButton.GetComponentInChildren<TextMeshProUGUI>().text = "invite";
        inviteButton.interactable = true;
      } else {
        inviteButton.GetComponentInChildren<TextMeshProUGUI>().text = "max team";
        inviteButton.interactable = false;
      }
      
    }

    public void Invite() {
      if(selectedHero) {
        selectedHero.invited = !selectedHero.invited;
        inviteButton.GetComponentInChildren<TextMeshProUGUI>().text = selectedHero.invited?"kick":"invite";
        selectedHero.GetComponent<Image>().color = selectedHero.invited ? selectedColor : defaultColor;
        heroesCount += selectedHero.invited ? 1 : -1;
        if(selectedHero.invited) {
          activeHeroes.Add(selectedHero);
        } else {
          activeHeroes.Remove(selectedHero);
        }
      }
    }

    public void Ready() {

    }
}
