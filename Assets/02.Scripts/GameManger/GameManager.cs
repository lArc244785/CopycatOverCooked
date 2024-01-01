using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

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

		[SerializeField]
		private string m_SceneName;
		[SerializeField]
		private string m_ResulteSceneName;

		public int StageClearScore;

		private void Awake()
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		[ServerRpc(RequireOwnership = false)]
		public void StartGameServerRpc()
		{
			if (IsServer && !string.IsNullOrEmpty(m_SceneName))
			{
				state.Value = GameFlow.Load;
				var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
				if (status != SceneEventProgressStatus.Started)
				{
					Debug.LogWarning($"Failed to load {m_SceneName} " +
						  $"with a {nameof(SceneEventProgressStatus)}: {status}");
				}
			}
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
