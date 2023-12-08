using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.Utensils
{
    public class UIProgressBar : MonoBehaviour
    {
        public float maxValue;
        public float minValue;
        private float currentValue;

        [SerializeField] private Image img_Gauge;

		public void Awake()
		{
			
		}


		public float GetGauge()
        {
            float value = Mathf.Clamp(currentValue, minValue, maxValue);
            return value / maxValue;
		}

	}
}
