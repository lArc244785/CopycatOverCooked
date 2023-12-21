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
		public void DropServerRpc(ulong clientID = ulong.MaxValue)
		{
			NetworkObject.TrySetParent(default(Transform));
			pickingClientID.Value = EMPTY_CLIENT_ID;
		}

		protected abstract void OnEndInteraction(IInteractable other);

		private IInteractable DetectInteractable()
		{
			// todo -> cast interactable.
			var interactionObjects = Physics.OverlapSphere(transform.position, _detectRadius, _layerMask);
			IInteractable select = null;
			foreach (var interactable in interactionObjects)
			{
				IInteractable item = interactable.GetComponent<IInteractable>();
				if (item == null)
					throw new Exception($"IIteractable Not found  {interactable.name}");
				if (select == null)
					select = item;
				else if (select.type < item.type)
				{
					select = item;
				}
			}

			return select;
		}

		[ServerRpc(RequireOwnership = false)]
		public void DestoryObjectServerRpc()
		{
			GetComponent<NetworkObject>().Despawn();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, _detectRadius);
		}
	}
}
