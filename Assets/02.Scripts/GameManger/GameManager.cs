using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace CopycatOverCooked.GamePlay
{
	public enum GameFlow
	{
		None,
		Lobby,
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

		private void Awake()
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public override void OnNetworkSpawn()
		{
			if (IsServer && !string.IsNullOrEmpty(m_SceneName))
			{
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

		[ClientRpc]
		public void GameStartClientRpc()
		{
			if (Interactor.spawned[OwnerClientId].TryGetComponent<Rigidbody>(out var rig))
				rig.isKinematic = false;
			if (Interactor.spawned[OwnerClientId].TryGetComponent<ClientBehaviour>(out var player))
				player.isControl = true;
		}

		private void Update()
		{
			Debug.Log(IsServer);
		}
	}
}
