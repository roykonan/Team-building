using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInAndChangeScene : MonoBehaviour
{
    public float time;
    private float timer;
    public string sceneName;
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        image.color = new Color(image.color.r,image.color.g,image.color.b,timer/time);
        if(timer>=time) {
          SceneManager.LoadScene(sceneName);
        }
    }
}
