using CopycatOverCooked.NetWork;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.GamePlay
{
	public class Interactor : NetworkBehaviour
	{
		public const ulong NETWORK_OBJECT_NULL_ID = ulong.MaxValue;
		public static Dictionary<ulong, Interactor> spawned = new Dictionary<ulong, Interactor>();

		public Transform hand;
		public IUsable currentUseable;
		public NetworkVariable<ulong> currentInteractableNetworkObjectID = new NetworkVariable<ulong>();

		[SerializeField] private Vector3 _offset;
		[SerializeField] private float _maxDistance;
		[SerializeField] private LayerMask _layerMask;

		private Vector3 offsetPoint => transform.position +
				   transform.right * _offset.x +
				   transform.up * _offset.y +
				   transform.forward * _offset.z;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			spawned.Add(OwnerClientId, this);

			Debug.Log($"{gameObject.name} is ClinetID {OwnerClientId}");

			if (IsServer)
				currentInteractableNetworkObjectID.Value = NETWORK_OBJECT_NULL_ID;
		}

		private void Update()
		{
			if (!IsOwner)
				return;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				// 상호작용하고있는게 없을때
				if (currentInteractableNetworkObjectID.Value == NETWORK_OBJECT_NULL_ID)
				{
					var detect = DetectInteractable();
					detect?.BeginInteraction(this);
				}
				// 상호작용중인게 있을때
				else if (TryCurrentGetInteractable(out var interactable))
				{
					interactable.EndInteraction(this);
				}
			}

		}



		private IInteractable DetectInteractable()
		{
			// todo -> cast interactable.
			var origin = offsetPoint;
			IInteractable select = null;

			RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, _maxDistance, _layerMask);
			foreach(var hit in hits)
			{
				ulong id = hit.collider.GetComponent<NetworkObject>().NetworkObjectId;
				IInteractable item = hit.collider.GetComponent<IInteractable>();
				if (item == null)
					throw new Exception($"IIteractable Not found  {hit.collider.name}");
				if (select == null)
					select = item;
				else if (id != currentInteractableNetworkObjectID.Value &&
						 select.type < item.type)
					select = item;
			}

			return select;
		}



		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			var startPos = offsetPoint;
			var endPos = startPos + Vector3.down * _maxDistance;
			Gizmos.DrawLine(startPos, endPos);
		}

		public bool TryCurrentGetInteractable(out IInteractable interactable)
		{
			interactable = null;
			if (this.TryGet(currentInteractableNetworkObjectID.Value, out var no))
			{
				if (no.TryGetComponent<IInteractable>(out interactable))
				{
					return true;
				}
			}

			return false;
		}
	}
}
