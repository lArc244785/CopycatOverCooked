using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using CopycatOverCooked.Orders;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Object
{

	public class ServingConvayer : NetworkBehaviour, IInteractable
	{
		public InteractableType type => InteractableType.ServingConvayer;
		private NetworkList<ulong> _outputPlateObjectIDs;
		[SerializeField] private OrderManager _manager;
		[SerializeField] private Transform _outputPoint;
		[SerializeField] private float _returnTick;

		private void Awake()
		{
			_outputPlateObjectIDs = new NetworkList<ulong>();
		}

		public void BeginInteraction(Interactor interactor)
		{
			if (_outputPlateObjectIDs.Count == 0)
				return;

			int topPlateIndex = _outputPlateObjectIDs.Count - 1;
			if (this.TryGet(_outputPlateObjectIDs[topPlateIndex], out var plateObject))
			{
				if (plateObject.TryGetComponent<Plate>(out var plate))
				{
					plate.PickUpServerRpc(interactor.OwnerClientId);
					_outputPlateObjectIDs.RemoveAt(topPlateIndex);
				}
			}
		}

		public void EndInteraction(Interactor interactor)
		{
		}


		public void SendOrder(Plate plate)
		{
			IngredientType orderSendIngredient = plate.GetIngereint();
			SendOrderServerRpc(plate.NetworkObjectId, (int)orderSendIngredient);
			plate.SpillIngredientServerRpc();

		}

		[ServerRpc(RequireOwnership = false)]
		private void SendOrderServerRpc(ulong networkObjectID, int ingredientType)
		{
			_manager.DeliveryServerRpc((IngredientType)ingredientType);
			StartCoroutine(ReturnWait(networkObjectID));
		}

		private IEnumerator ReturnWait(ulong plateObjectID)
		{
			if (this.TryGet(plateObjectID, out var plate))
			{
				plate.gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(_returnTick);
			plate.gameObject.SetActive(true);
			PlateToOutputPointServerRpc(plateObjectID);
		}

		[ServerRpc(RequireOwnership = false)]
		private void PlateToOutputPointServerRpc(ulong plateObjectId)
		{
			if (this.TryGet(plateObjectId, out var networkObject))
			{
				if (networkObject.TrySetParent(transform))
				{
					networkObject.transform.localPosition = _outputPoint.localPosition;
					networkObject.transform.localRotation = _outputPoint.localRotation;
					if (_outputPlateObjectIDs.Count > 0)
					{
						if (this.TryGet(_outputPlateObjectIDs[_outputPlateObjectIDs.Count - 1], out var topObject))
						{
							networkObject.transform.localPosition = topObject.transform.localPosition + Vector3.up * 0.5f;
						}

					}
					_outputPlateObjectIDs.Add(plateObjectId);
				}
			}
		}

	}
}