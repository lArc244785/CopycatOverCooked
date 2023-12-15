using CopycatOverCooked.Datas;
using CopycatOverCooked.NetWork.Untesils;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.UIs
{
    public class SinkUI : MonoBehaviour
    {
        [SerializeField] private GameObject _progressBar;
        [SerializeField] private Image _progressGague;

        private NetUtensillBase _utensil;

        private void Start()
        {
            _utensil = transform.root.GetComponent<NetUtensillBase>();
            _utensil.onChangeProgress += UpdateProgress;

            _progressBar.SetActive(false);
        }

        private void UpdateProgress(float current, float surcessProgress)
        {
            Debug.Log(current);
            if (current <= 0.0f)
            {
                _progressBar.SetActive(false);
                return;
            }

            _progressBar.SetActive(true);

            float fillAmount = Mathf.Clamp(current / surcessProgress, 0.0f, 1.0f);
            _progressGague.fillAmount = fillAmount;
        }
    }
}