using CopycatOverCooked.NetWork;
using System;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.GamePlay
{
	public abstract class Pickable : NetworkBehaviour, IInteractable
	{
		protected NetworkVariable<ulong> pickingClientID = new NetworkVariable<ulong>(EMPTY_CLIENT_ID);
		const ulong EMPTY_CLIENT_ID = ulong.MaxValue;

		[SerializeField] private float _detectRadius;
		[SerializeField] private LayerMask _layerMask;

		public abstract InteractableType type { get; }

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			if (IsServer == false)
				return;
		}

		public void BeginInteraction(Interactor interactor)
		{
			if (pickingClientID.Value != EMPTY_CLIENT_ID)
				return;

			PickUpServerRpc(interactor.OwnerClientId);
		}

		public void EndInteraction(Interactor interactor)
		{
			if (pickingClientID.Value == EMPTY_CLIENT_ID)
				return;

			var detect = DetectInteractable();
			if (detect == null)
				return;

			OnEndInteraction(DetectInteractable());
		}

		[ServerRpc(RequireOwnership = false)]
		public void PickUpServerRpc(ulong clientID)
		{
			Interactor interactor = Interactor.spawned[clientID];
			// Pick up
			if (NetworkObject.TrySetParent(interactor.NetworkObject))
			{
				transform.localPosition = interactor.hand.localPosition;
				pickingClientID.Value = clientID;
				interactor.currentInteractableNetworkObjectID.Value = NetworkObjectId;
			}
		}

		[ServerRpc(RequireOwnership = false)]
		public void DropServerRpc()
		{
			if (pickingClientID.Value == Pickable.EMPTY_CLIENT_ID)
				return;

			Interactor interactor = Interactor.spawned[pickingClientID.Value];

			NetworkObject.TrySetParent(default(Transform));
			pickingClientID.Value = EMPTY_CLIENT_ID;
			interactor.currentInteractableNetworkObjectID.Value = Interactor.NETWORK_OBJECT_NULL_ID;
		}

		protected abstract void OnEndInteraction(IInteractable other);

		private IInteractable DetectInteractable()
		{
			// todo -> cast interactable.
			var interactionObjects = Physics.OverlapSphere(transform.position, _detectRadius, _layerMask);
			IInteractable select = null;
			foreach (var interactable in interactionObjects)
			{
				if(interactable.TryGetComponent<NetworkObject>(out var networkObject))
				{
					IInteractable item = interactable.GetComponent<IInteractable>();
					if (item == null)
						throw new Exception($"IIteractable Not found  {interactable.name}");
					if (select == null)
						select = item;
					else if (networkObject.NetworkObjectId != NetworkObjectId &&
							 select.type < item.type)
					{
						select = item;
					}
				}
			}

			return select;
		}

		[ServerRpc(RequireOwnership = false)]
		public void DestoryObjectServerRpc()
		{
			GetComponent<NetworkObject>().Despawn();
			Debug.Log("Object Destory");
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, _detectRadius);
		}

	}
}
