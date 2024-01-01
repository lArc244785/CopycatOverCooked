using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _roomName = null;
    [SerializeField] private Button _checkBox = null;
    [SerializeField] private Button _cancle = null;
    [SerializeField] private Button _create = null;
    [SerializeField] private Transform _check = null;
    [SerializeField] private TMP_InputField _passWord = null;
    public static CreateLobbyUI Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            if(_instance == null)
            {
                _instance = new CreateLobbyUI();
            }
        }
    }
    private static CreateLobbyUI _instance;

    private bool _ischeck = false;

    private void Awake()
    {
        _cancle.onClick.AddListener(Cancle);
        _create.onClick.AddListener(Create);
        _checkBox.onClick.AddListener(Check);
    }

    private void Cancle()
    {
        this.gameObject.SetActive(false);

        _roomName.text = "";
        _passWord.text = "";
    }

    private void Create()
    {
        LobbyManager.Instance.CreateLobby(_roomName.text, 4, _ischeck, _passWord.text);
    }

    private void Check()
    {
        _ischeck = _ischeck? false : true;

        _check.gameObject.SetActive(_ischeck);
        _passWord.gameObject.SetActive(_ischeck);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}