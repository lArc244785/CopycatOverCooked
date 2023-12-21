using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatSample : NetworkBehaviour
{
	private NetworkVariable<int> m_userCount = new NetworkVariable<int>();

	[SerializeField] private TextMeshProUGUI _log;
	[SerializeField] private TMP_InputField _inputField;
	[SerializeField] private TextMeshProUGUI _userCountText;

	private StringBuilder _builder  = new(1000);

	private List<string> _chatLogList = new List<string>(10);

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		m_userCount.OnValueChanged += (prev, current) => OnChangeUserCountClientRpc(current);
		ConnetServerRpc();
		_log.text = string.Empty;
	}

	private void OnApplicationQuit()
	{
		DisConnetServerRpc();
	}


	[ClientRpc]
	private void OnChangeUserCountClientRpc(int count)
	{
		_userCountText.text = $"UserCount : {count}";
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (_inputField.text == string.Empty)
				return;

			ChatSendServerRpc(_inputField.text);
			_inputField.text = string.Empty;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void ChatSendServerRpc(string message, ServerRpcParams parms = default)
	{
		message = $"[{parms.Receive.SenderClientId}] {message}";
		ChatReceveClientRpc(message);
	}

	[ClientRpc]
	private void ChatReceveClientRpc(string message, ClientRpcParams parms = default)
	{
		_builder.Clear();
		if (_chatLogList.Count == _chatLogList.Capacity)
			_chatLogList.RemoveAt(0);

		_chatLogList.Add(message);

		foreach(var item in _chatLogList)
		{
			_builder.AppendLine(item);
		}

		_log.text = _builder.ToString();
	}

	[ServerRpc(RequireOwnership =false)]
	private void ConnetServerRpc()
	{
		m_userCount.Value++;
	}


	[ServerRpc(RequireOwnership = false)]
	private void DisConnetServerRpc()
	{
		m_userCount.Value--;
	}

}
