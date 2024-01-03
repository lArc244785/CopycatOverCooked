using Cinemachine;
using System;
using System.Collections;
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
		public bool isTest = true;

		[SerializeField] private CinemachineVirtualCamera _playerCam;

		[SerializeField] private Transform[] _startPoints;

		public int PlayTime;
		public NetworkVariable<int> InGameTime = new NetworkVariable<int>();
		public Action<int> onChangeInGameTime;

		public int goalScore;
		public NetworkVariable<int> currentScore = new NetworkVariable<int>(0);
		public Action<int> onChangeGoalScore;

		private void Awake()
		{
			instance = this;
			InGameTime.OnValueChanged += (p, c) => onChangeInGameTime?.Invoke(c);
			currentScore.OnValueChanged += (p, c) => onChangeGoalScore?.Invoke(c);
		}

		private void Start()
		{
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			currentStep.OnValueChanged += InitProgress;
			if (IsServer)
			{
				InGameTime.Value = PlayTime;
				currentStep.Value = Step.WaitUntilAllPlayersAreReady;
			}

			if (isTest == false)
			{
				GameManager.instance.InGameSceneCompleteServerRpc();
			}

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
					if (IsServer)
					{
                        int count = 0;
                        foreach (var item in ClientBehaviour.spawned)
                        {
							Vector3 startPosition = GetStartPoint(count++);
                            item.Value.SetPositionAndRotationClientRpc(startPosition, Vector3.zero);
                            item.Value.Active();
                        }
                    }
					Debug.Log("모든 플레이어가 준비가 완료되어 스테이지 시작전 해야되는 내용을 시작합니다.");
					CameraSetting();
					currentStep.Value = Step.StartStage;
					break;
				case Step.StartStage:
					if (IsServer)
					{
						GameManager.instance.GameStartServerRpc();
						currentStep.Value = Step.DuringStartStage;
					}

					break;
				case Step.AfterStartStage:
					break;
				case Step.DuringStartStage:
					if (IsServer)
					{
						StartCoroutine(C_GameTimer());
					}
					break;
			}
		}

		private void CameraSetting()
		{
			Interactor interactor = Interactor.spawned[OwnerClientId];
			_playerCam.Follow = interactor.transform;
		}

		public Vector3 GetStartPoint(int index)
		{
			return _startPoints[index].position;
		}

		private IEnumerator C_GameTimer()
		{
			while (InGameTime.Value > 0.0f)
			{
				yield return new WaitForSeconds(1.0f);
				InGameTime.Value--;
			}

			GameManager.instance.GameOverServerRpc(currentScore.Value);


		}

	}
}