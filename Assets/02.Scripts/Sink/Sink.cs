using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using Unity.Netcode;
using UnityEngine;
using Progress = CopycatOverCooked.Datas.Progress;

namespace CopycatOverCooked.Object
{

	public class Sink : NetworkBehaviour, IInteractable, IUsable
	{
		[SerializeField] private float _washingTime = 3f;
		private NetworkVariable<float> _currentTime = new NetworkVariable<float>(0.0f);
		private NetworkVariable<Progress> _progress = new NetworkVariable<Progress>(Progress.None);

		[SerializeField] private Transform _dirtyPlatePoint;
		[SerializeField] private Transform _clearPlatePoint;

		private NetworkVariable<ulong> _plateNetworkObjectID = new NetworkVariable<ulong>(NULL_OBJECT_ID);

		private const ulong NULL_OBJECT_ID = ulong.MaxValue;

		public InteractableType type => InteractableType.Sink;

		private void Awake()
		{

		}


		public void BeginInteraction(Interactor interactor)
		{
			if (_progress.Value == Progress.Sucess)
			{
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
		}

		public void EndInteraction(Interactor interactor)
		{

		}

		public void Use(NetworkObject user)
		{
			switch (_progress.Value)
			{
				case Progress.Progressing:
					_currentTime.Value += Time.deltaTime;
					if (_currentTime.Value >= _washingTime)
						PlateClearServerRpc();
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
			if (other.tag == "Player")
			{
				if (other.TryGetComponent<Interactor>(out var player))
				{
					player.SetUsableObejctClientRpc(NetworkObjectId);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
			{
				if (other.TryGetComponent<Interactor>(out var player))
				{
					if(player.currentUsable == (IUsable)this)
					{
						player.currentUsable = null;
					}
				}
			}
		}
	}
}
