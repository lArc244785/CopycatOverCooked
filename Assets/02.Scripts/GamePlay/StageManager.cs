using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.GamePlay
{


	public class StageManager : NetworkBehaviour
	{
		public enum Step
		{
			Idle,
			WaitUntilAllPlayersAreReady,
			BeforeStartStage,
			StartStage,
			AfterStartStage,
			DuringStartStage,
		}
		public static StageManager instance;
		public NetworkVariable<Step> currentStep = new NetworkVariable<Step>(Step.Idle);

		private void Awake()
		{
			instance = this;

		}

		private void Start()
		{
			if (GameManager.instance.IsServer)
			{
				currentStep.OnValueChanged += InitProgress;
			}
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			if (IsServer)
			{
				currentStep.OnValueChanged += InitProgress;
			}

			GameManager.instance.InGameSceneCompleteServerRpc();
		}

		private void InitProgress(Step prev, Step current)
		{
			switch (current)
			{
				case Step.Idle:
					break;
				case Step.WaitUntilAllPlayersAreReady:
					break;
				case Step.BeforeStartStage:
					Debug.Log("모든 플레이어가 준비가 완료되어 스테이지 시작전 해야되는 내용을 시작합니다.");
					currentStep.Value = Step.StartStage;
					break;
				case Step.StartStage:
					GameManager.instance.GameStartClientRpc();
					break;
				case Step.AfterStartStage:
					break;
				case Step.DuringStartStage:
					break;
			}

		}
	}
}