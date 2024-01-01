using CopycatOverCooked.GamePlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResulteUI : MonoBehaviour
{
	[SerializeField] private Image[] stars;

	private void Start()
	{
		var score = GameManager.instance.StageClearScore;
		if (score >= 1000)
			stars[2].color = Color.white;
		if(score >= 500 )
			stars[1].color = Color.white;
		if( score >= 100 )
			stars[0].color = Color.white;
	}
}
