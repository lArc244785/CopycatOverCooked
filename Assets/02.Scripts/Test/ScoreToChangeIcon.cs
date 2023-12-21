using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreToChangeIcon : MonoBehaviour
{

    [SerializeField] public float targetScore;
    [SerializeField] public Text targetScoreText;

    public float gameScore;
    public GameObject starObject;
    public Image star;


    void Start()
    {
        star = starObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetScore <= gameScore)
        {
            star.color = new Color32(255,255,255,255);
        }
        
    }
}
