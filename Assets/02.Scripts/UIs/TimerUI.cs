using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CopycatOverCooked.GamePlay;

namespace CopycatOverCooked.UIs
{

	public class TimerUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _text;
		[SerializeField] private Slider _slider;

		private void Start()
		{
			_slider.maxValue = StageManager.instance.PlayTime;
			_slider.minValue = 0.0f;
			StageManager.instance.onChangeInGameTime += OnChangeGameTime;
			
		}

		private void OnChangeGameTime(int time)
		{
			float min = time / 60;
			float sce = time % 60;
			
			_slider.value = time;
			_text.text = $"{min} : {sce}";

			_slider.value = time;
		}
	}
}
