using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class Table : NetworkBehaviour, IInteractable
{
	[SerializeField] private Transform _putPoint;

	public InteractableType type => InteractableType.Table;

	[SerializeField] private NetworkVariable<ulong> _putNetworkObjectId = new NetworkVariable<ulong>(EMPTY_PUT_OBJECT);

	public const ulong EMPTY_PUT_OBJECT = ulong.MaxValue;

	[SerializeField] private List<InteractableType> _putableObjectList = new List<InteractableType>();

	protected event Action<ulong> onChangeputNetworkObject;

	[SerializeField] protected LayerMask _layerMask;

	protected virtual void Awake()
	{
		_putNetworkObjectId.OnValueChanged = OnChangeputNetworkObject;
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsServer == false)
			return;

		StartCoroutine(C_NetworkInit());
	}

	private IEnumerator C_NetworkInit()
	{
		//yield return new WaitUntil(() => StageManager.instance.current == StageManager.Step.BeforeStartStage);

		yield return new WaitForSeconds(1.0f);
		var hits = Physics.RaycastAll(transform.position, Vector3.up, 10.0f, _layerMask);
		foreach (var hit in hits)
		{
			if (hit.collider.TryGetComponent<NetworkObject>(out var networkObject))
			{
				if (networkObject.NetworkObjectId != NetworkObjectId)
				{
					PutObjectServerRpc(networkObject.NetworkObjectId);
					break;
				}
			}
		}
	}

	/// <summary>
	/// Only ServerCall
	/// </summary>
	private void OnChangeputNetworkObject(ulong prev, ulong current)
	{
		if (IsServer == false)
			return;
		onChangeputNetworkObject?.Invoke(current);
	}

	public void BeginInteraction(Interactor interactor)
	{
		if (this.TryGet(_putNetworkObjectId.Value, out var putObject))
		{
			if(putObject.TryGetComponent<Pickable>(out var  pickable))
			{
				PickUpObjectServerRpc(interactor.OwnerClientId);
			}
			else if(putObject.TryGetComponent<IInteractable>(out var interactable))
			{
				interactable.BeginInteraction(interactor);
			}
		}
	}

	public void EndInteraction(Interactor interactor)
	{
	}

	[ServerRpc(RequireOwnership = false)]
	private void PickUpObjectServerRpc(ulong clientID)
	{
		Interactor interactor = Interactor.spawned[clientID];

		if (_putNetworkObjectId.Value != EMPTY_PUT_OBJECT &&
			interactor.currentInteractableNetworkObjectID.Value == Interactor.NETWORK_OBJECT_NULL_ID)
		{
			if (this.TryGet(_putNetworkObjectId.Value, out var netObject))
			{
				if (netObject.TryGetComponent<Pickable>(out var pickableObject))
				{
					pickableObject.PickUpServerRpc(clientID);
					_putNetworkObjectId.Value = EMPTY_PUT_OBJECT;
				}
			}
		}
	}


	public bool CanPutObject(InteractableType type)
	{
		foreach (var putableType in _putableObjectList)
		{
			if (putableType == type && _putNetworkObjectId.Value == EMPTY_PUT_OBJECT)
				return true;
		}
		return false;
	}

	[ServerRpc(RequireOwnership = false)]
	public void PutObjectServerRpc(ulong networkObjectID)
	{
		if (this.TryGet(networkObjectID, out var netObject))
		{
			if (netObject.TrySetParent(transform))
			{
				netObject.transform.localPosition = _putPoint.localPosition;
				netObject.transform.localRotation = Quaternion.identity;
				_putNetworkObjectId.Value = networkObjectID;
			}
		}
	}

	public bool TryGetPutObject(out NetworkObject netobject)
	{
		netobject = null;

		if (_putNetworkObjectId.Value == EMPTY_PUT_OBJECT)
			return false;

		if (this.TryGet(_putNetworkObjectId.Value, out netobject))
			return true;

		return false;
	}

	[ServerRpc(RequireOwnership = false)]
	public void PopPutObjectServerRpc()
	{
		if (TryGetPutObject(out var putObject))
		{
			if (putObject.TrySetParent(default(Transform)))
			{
				_putNetworkObjectId.Value = EMPTY_PUT_OBJECT;
			}
		}
	}

}
