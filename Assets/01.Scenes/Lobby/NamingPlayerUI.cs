using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NamingPlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private Button _enterButton;
    [SerializeField] private Transform _joinAndCreateUI;

    private void Awake()
    {
        _enterButton.onClick.AddListener(CreateName);
    }

    private void CreateName()
    {
        if(!string.IsNullOrWhiteSpace(_playerName.text) && _playerName.text.Length < 15)
        {
            string replaceBlank = _playerName.text.Replace(" ", "");

            LobbyManager.Instance.Authenticate(replaceBlank);

            this.gameObject.SetActive(false);
            _joinAndCreateUI.gameObject.SetActive(true);
        }

        else
        {
            _playerName.text = "";
        }
    }
}
