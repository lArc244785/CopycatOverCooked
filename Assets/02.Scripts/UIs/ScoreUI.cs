using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CopycatOverCooked.GamePlay;

namespace CopycatOverCooked.UIs
{
	public class ScoreUI : MonoBehaviour
	{
		[SerializeField] private Slider _slider;
		[SerializeField] protected TextMeshProUGUI _text;

		private void Start()
		{
			_slider.maxValue = StageManager.instance.goalScore;
			_slider.minValue = 0;

			StageManager.instance.onChangeGoalScore += OnChangeScore;
		}

		private  void OnChangeScore(int score)
		{
			_slider.value = score;
			_text.text = score.ToString();
		}

	}
}

