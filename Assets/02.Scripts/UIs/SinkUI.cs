using CopycatOverCooked.Datas;
using CopycatOverCooked.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.UIs
{
    public class SinkUI : MonoBehaviour
    {
		[SerializeField] private Slider _slider;
		[SerializeField] private Sink _sink;

		private void Start()
		{
			_slider.minValue = 0.0f;
			_slider.maxValue = _sink.washingTime;

			_sink.onChangeCurrentTime += OnChangedateTime;
			_sink.onChangeProgress += OnChangeProgress;
			OnChangeProgress(Progress.None);
		}

		private void OnChangedateTime(float time)
		{
			_slider.value = time;
		}

		private void OnChangeProgress(Progress progress)
		{
			bool isActive = progress == Progress.Progressing ? true : false;
			_slider.gameObject.SetActive(isActive);
		}
	}
}
