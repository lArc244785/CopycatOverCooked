using CopycatOverCooked.Datas;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked
{
	public class Plate : NetworkBehaviour, ISpill
	{
		[field: SerializeField] public int capacity { private set; get; }

		#region NetVailable
		public NetworkList<int> inputIngredients;
		public NetworkVariable<bool> isDirty = new NetworkVariable<bool>();
		#endregion

		public event Action<IEnumerable<IngredientType>> onChangeSlot;
		public event Action<bool> onChangeDirty;
		private event Action<IngredientType> onChangeResult;

		public IngredientType result
		{
			private set
			{
				_result = value;
				onChangeResult?.Invoke(_result);
			}
			get
			{
				return _result;
			}
		}
		private IngredientType _result;

		[SerializeField] private Transform _visualDataPoint;
		private GameObject _visualObject;

		private void Awake()
		{
			inputIngredients = new NetworkList<int>();
			inputIngredients.OnListChanged += OnChangeSlot;
			onChangeResult += UpdateVisualData;
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			InitCallSlotEvent();
		}

		private void InitCallSlotEvent()
		{
			IngredientType[] slots = new IngredientType[inputIngredients.Count];
			for (int i = 0; i < slots.Length; i++)
				slots[i] = (IngredientType)inputIngredients[i];

			onChangeSlot?.Invoke(slots);
		}


		private void OnChangeSlot(NetworkListEvent<int> changeEvent)
		{
			IngredientType[] slots = new IngredientType[changeEvent.Index + 1];

			result = IngredientType.None;

			for (int i = 0; i < changeEvent.Index; i++)
			{
				slots[i] = (IngredientType)inputIngredients[i];
			}
			slots[changeEvent.Index] = (IngredientType)changeEvent.Value;

			foreach (var ingredient in slots)
				result |= ingredient;

			onChangeSlot?.Invoke(slots);
		}


		public void AddIngredient(params IngredientType[] ingredient)
		{
			if (IsServer == false || ingredient == null)
				return;

			for (int i = 0; i < ingredient.Length && inputIngredients.Count < capacity; i++)
			{
				inputIngredients.Add((int)ingredient[i]);
				Debug.Log(ingredient[i]);
			}
		}


		[ServerRpc(RequireOwnership = false)]
		public void WashServerRpc()
		{
			isDirty.Value = false;
		}

		[ServerRpc(RequireOwnership = false)]
		public void EmptyServerRpc()
		{
			inputIngredients.Clear();
			isDirty.Value = true;
		}

		public IngredientType[] Spill()
		{
			IngredientType[] resources = new IngredientType[inputIngredients.Count];
			for (int i = 0; i < resources.Length; i++)
				resources[i] = (IngredientType)inputIngredients[i];

			inputIngredients.Clear();
			isDirty.Value = true;

			return resources;
		}

		private void UpdateVisualData(IngredientType result)
		{
			if (_visualObject != null)
			{
				Destroy(_visualObject);
				_visualObject = null;
			}

			if (inputIngredients.Count == 0)
				return;

			var prefab = IngredientVisualDataDB.instance.GetPrefab(result);
			_visualObject = Instantiate(prefab);
			_visualObject.transform.parent = transform;
			_visualObject.transform.localPosition = _visualDataPoint.localPosition;
		}
	}
}
