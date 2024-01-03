using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;

namespace CopycatOverCooked.GamePlay
{
	public enum GameFlow
	{
		None,
		Lobby,
		Load,
		Play,
		Gameover,
	}

	public class GameManager :NetworkBehaviour 
	{
		public NetworkVariable<GameFlow> state = new NetworkVariable<GameFlow>();
		public static GameManager instance;

		public int loadPlayCount = 2;
		private NetworkVariable<int> _waitPlayerCount = new NetworkVariable<int>(0);


        public event EventHandler OnFailedToJoinGame;

        [SerializeField]
		private string m_SceneName;
		[SerializeField]
		private string m_ResulteSceneName;

		public int StageClearScore;

		private void Awake()
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
            //NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
			//OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        }

		public bool startcli()
		{
            if (NetworkManager.Singleton.StartClient())
            {
                return true;
            }

            else
            {
                return false;
            }
        }

		public bool startho()
		{
			if(NetworkManager.Singleton.StartHost())
			{
				return true;
			}

			else 
			{
				return false; 
			}
		}

		[ServerRpc(RequireOwnership = false)]
		public void StartGameServerRpc(string sceneName)
		{
			TestClientRpc(sceneName);

            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            //if (IsServer && !string.IsNullOrEmpty(sceneName))
            //{
            //	state.Value = GameFlow.Load;

            //	Debug.Log("¾Àº¯°æ");

            //             var status = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            //	if (status != SceneEventProgressStatus.Started)
            //	{
            //		Debug.LogWarning($"Failed to load {sceneName} " +
            //			  $"with a {nameof(SceneEventProgressStatus)}: {status}");
            //	}
            //}
        }

        private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
        {
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
        }

		private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
		{
			if (NetworkManager.Singleton.DisconnectReason == "")
			{
                Debug.Log("Failed to connect");
			}
			else
			{
				Debug.Log(NetworkManager.Singleton.DisconnectReason);
			}
		}

            [ClientRpc]
		private void TestClientRpc(string sceneName)
		{
			Debug.Log("PingPong");

            //var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);

			SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }


		public bool IsPlayerLoadFish()
		{
			return loadPlayCount == _waitPlayerCount.Value;
		}

		[ServerRpc(RequireOwnership = false)]
		public void InGameSceneCompleteServerRpc()
		{
			_waitPlayerCount.Value++;
			if(_waitPlayerCount.Value == loadPlayCount)
			{
				StageManager.instance.currentStep.Value = StageManager.Step.BeforeStartStage;
			}
		}

		[ServerRpc(RequireOwnership = false)]
		public void GameStartServerRpc()
		{
			state.Value = GameFlow.Play;
		}

		[ServerRpc(RequireOwnership =false)]
		public void GameOverServerRpc(int value)
		{
			state.Value = GameFlow.Gameover;
			StageClearScore = value;

			if (IsServer && !string.IsNullOrEmpty(m_SceneName))
			{
				state.Value = GameFlow.Load;
				var status = NetworkManager.SceneManager.LoadScene(m_ResulteSceneName, LoadSceneMode.Single);
				if (status != SceneEventProgressStatus.Started)
				{
					Debug.LogWarning($"Failed to load {m_ResulteSceneName} " +
						  $"with a {nameof(SceneEventProgressStatus)}: {status}");
				}
			}
		}
	}
}
