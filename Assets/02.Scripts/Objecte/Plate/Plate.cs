using CopycatOverCooked.Datas;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace CopycatOverCooked
{
	public class Plate : NetworkBehaviour
	{
		[field: SerializeField] public int capacity { private set; get; }

		#region NetVailable
		public NetworkList<int> inputIngredients;
		public NetworkVariable<bool> isDirty = new NetworkVariable<bool>();
		#endregion

		public event Action<IEnumerable<IngredientType>> onChangeSlot;
		public event Action<bool> onChangeDirty;

		public IngredientType result { private set; get; }

		public ulong owner => _owner;

		private ulong _owner = 101;

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

		private void Awake()
		{
			inputIngredients = new NetworkList<int>();
			inputIngredients.OnListChanged += OnChangeSlot;
			//isDirty.OnValueChanged += (prev, current) => onChangeDirty(current);
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

		
		public void AddIngredient(IngredientType[] ingredient)
		{
			if (IsServer == false)
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


	}
}
