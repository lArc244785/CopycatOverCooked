using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.GamePlay
{
    public class Interactor : NetworkBehaviour
    {
        public static Dictionary<ulong, Interactor> spawned = new Dictionary<ulong, Interactor>();

        public Transform hand;
        public IInteractable currentInteractable;
        public IUsable currentUseable;

        [SerializeField] private Vector3 _offset;
		[SerializeField] private float _detectRadius;
        [SerializeField] private float _distance;
        [SerializeField] private LayerMask _layerMask;

		public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            spawned.Add(OwnerClientId, this);
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 상호작용하고있는게 없을때
                if (currentInteractable == null)
                {
                    currentInteractable = DetectInteractable();
                    currentInteractable?.BeginInteraction(this);
                }
                // 상호작용중인게 있을때
                else
                {
                    currentInteractable.EndInteraction(this);
                    currentInteractable = null;
                }
            }
               
        }

        private IInteractable DetectInteractable()
        {
			// todo -> cast interactable.
            var origin = transform.position + _offset + transform.forward * _distance;

			var interactionObjects = Physics.OverlapSphere(origin, _detectRadius, _layerMask);
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            var startPos = transform.position + _offset;
            var origin = transform.position + _offset + transform.forward * _distance;
            Gizmos.DrawLine(startPos, origin);
            Gizmos.DrawWireSphere(origin, _detectRadius);
		}
    }
}
