using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using System;
using Unity.Netcode;
using UnityEngine;
using Progress = CopycatOverCooked.Datas.Progress;

namespace CopycatOverCooked.Object
{

	public class Sink : NetworkBehaviour, IInteractable, IUsable
	{
		[SerializeField] public float washingTime { private set; get; } = 3.0f;
		private NetworkVariable<float> _currentTime = new NetworkVariable<float>(0.0f);
		private NetworkVariable<Progress> _progress = new NetworkVariable<Progress>(Progress.None);

		[SerializeField] private Transform _dirtyPlatePoint;
		[SerializeField] private Transform _clearPlatePoint;

		private NetworkVariable<ulong> _plateNetworkObjectID = new NetworkVariable<ulong>(NULL_OBJECT_ID);

		private const ulong NULL_OBJECT_ID = ulong.MaxValue;

		public InteractableType type => InteractableType.Sink;

		public event Action<float> onChangeCurrentTime;
		public event Action<Progress> onChangeProgress;
		public event Action onClear;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			_currentTime.OnValueChanged = (prev, current) => onChangeCurrentTime?.Invoke(current);
			_progress.OnValueChanged = (prev, current) => onChangeProgress?.Invoke(current);
		}


		public void BeginInteraction(Interactor interactor)
		{
			if (_progress.Value == Progress.Sucess)
			{
				PickupPlateServerRpc(interactor.OwnerClientId);
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void PickupPlateServerRpc(ulong clientID)
		{
			Interactor interactor = Interactor.spawned[clientID];

			if (this.TryGet(_plateNetworkObjectID.Value, out var clearPlateObject))
			{
				if (clearPlateObject.TryGetComponent<Plate>(out var plate))
				{
					plate.PickUpServerRpc(interactor.OwnerClientId);
					_progress.Value = Progress.None;
					_plateNetworkObjectID.Value = NULL_OBJECT_ID;
					_currentTime.Value = 0.0f;
				}
			}
		}

		public void EndInteraction(Interactor interactor)
		{

		}

		public void Use(NetworkObject user)
		{
			UseServerRpc();
		}

		[ServerRpc(RequireOwnership = false)]
		private void UseServerRpc()
		{
			switch (_progress.Value)
			{
				case Progress.Progressing:
					Debug.Log("설거지중...");
					_currentTime.Value += Time.deltaTime;
					if (_currentTime.Value >= washingTime)
					{
						PlateClearServerRpc();
						onClear?.Invoke();
					}
					break;
			}
		}


		public bool CanPlateToSink(Plate plate)
		{
			//들고있는 접시가 더러운 경우
			return plate.isDirty == true &&
					_progress.Value == Progress.None;
		}

		[ServerRpc(RequireOwnership = false)]
		public void PlateToSinkServerRpc(ulong plateNetworkObjectID)
		{
			if (this.TryGet(plateNetworkObjectID, out var plateObject))
			{
				if (plateObject.TrySetParent(transform))
				{
					plateObject.transform.localPosition = _dirtyPlatePoint.localPosition;
					plateObject.transform.localRotation = _dirtyPlatePoint.localRotation;
					_currentTime.Value = 0.0f;
					_progress.Value = Progress.Progressing;
					_plateNetworkObjectID.Value = plateNetworkObjectID;
				}
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void PlateClearServerRpc()
		{
			if (this.TryGet(_plateNetworkObjectID.Value, out var plateObject))
			{
				plateObject.transform.localPosition = _clearPlatePoint.localPosition;
				plateObject.transform.localRotation = _clearPlatePoint.localRotation;
				if (plateObject.TryGetComponent<Plate>(out var plate))
				{
					plate.ClearPlateClientRpc();
					_progress.Value = Progress.Sucess;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.root.TryGetComponent<Interactor>(out var player))
			{
				player.SetUsableObejctClientRpc(NetworkObjectId);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.transform.root.TryGetComponent<Interactor>(out var player))
			{
				if (player.currentUsable == (IUsable)this)
				{
					player.currentUsable = null;
				}
			}
		}
	}
}
