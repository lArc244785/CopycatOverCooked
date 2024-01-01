using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using CopycatOverCooked.Object;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Untesil
{
	public class PickUtensil : Pickable, IAddIngredient
	{
		public override InteractableType type => InteractableType.PickUtensil;
		[SerializeField] private List<CookRecipe> _cookableRecipeList;
		private NetworkVariable<Progress> _cookProgress = new NetworkVariable<Progress>();

		[field: SerializeField] public int capacity { private set; get; }
		private NetworkList<ulong> _ingredientObjectIDs;

		private NetworkVariable<float> _currentProgress = new NetworkVariable<float>(0.0f);
		[field: SerializeField] public float sucessProgress { private set; get; }
		[field: SerializeField] public float failProgress { private set; get; }

		[SerializeField] private Transform _ingredientPoint;

		[SerializeField] private Vector3 _progressOffset;
		[SerializeField] private Vector3 _sucessOffset;

		public event Action<IngredientType> onAddIngredient;
		public event Action<int, IngredientType> onChangeIngredinet;
		public event Action<int> onRemoveAtIngredientList;
		public event Action onFail;
		public event Action<float> onChangeProgress;

		private void Awake()
		{
			_ingredientObjectIDs = new NetworkList<ulong>();

			_ingredientObjectIDs.OnListChanged += OnChangeList;
			_currentProgress.OnValueChanged += (prev, curent) => onChangeProgress?.Invoke(curent);
		}

		private void OnChangeList(NetworkListEvent<ulong> changeEvent)
		{
			switch (changeEvent.Type)
			{
				case NetworkListEvent<ulong>.EventType.Add:
					if (this.TryGet(changeEvent.Value, out var networkObjcet))
					{
						if (networkObjcet.TryGetComponent<Ingredient>(out var addIngredient))
						{
							onAddIngredient?.Invoke(addIngredient.ingerdientType.Value);
						}
					}
					break;
				case NetworkListEvent<ulong>.EventType.RemoveAt:
					onRemoveAtIngredientList?.Invoke(changeEvent.Index);
					break;
			}
		}

		protected override void OnEndInteraction(IInteractable other)
		{
			switch (other.type)
			{
				case InteractableType.Ingredient:
					Ingredient ingredient = (Ingredient)other;
					if (CanAdd(ingredient.ingerdientType.Value))
						AddIngredientServerRpc(ingredient.NetworkObjectId);
					break;
				case InteractableType.Plate:
					if (_cookProgress.Value != Progress.Sucess)
						return;

					Plate plate = (Plate)other;
					DropIngredientToPlateServerRpc(plate.NetworkObjectId);
					break;
				case InteractableType.Table:
					Table table = (Table)other;
					if (table.CanPutObject(type))
					{
						DropServerRpc();
						table.PutObjectServerRpc(NetworkObjectId);
					}
					else if (table.TryGetPutObject(out var putObject))
					{
						if (putObject.TryGetComponent<Plate>(out var putPlate))
						{
							DropIngredientToPlateServerRpc(putObject.NetworkObjectId);
						}
						else if (putObject.TryGetComponent<Ingredient>(out var putIngredient))
						{
							if (CanAdd(putIngredient.ingerdientType.Value))
							{
								table.PopPutObjectServerRpc();
								AddIngredientServerRpc(putIngredient.NetworkObjectId);
							}

						}
					}
					break;
				case InteractableType.TrashCan:
					ClearIngredientServerRpc();
					break;
			}
		}

		#region Add Ingredient

		public bool CanAdd(IngredientType type)
		{
			if (_ingredientObjectIDs.Count >= capacity)
				return false;
			if (_cookProgress.Value == Progress.Fail)
				return false;

			foreach (var recipe in _cookableRecipeList)
			{
				if (type == recipe.source)
					return true;
			}
			return false;
		}

		[ServerRpc(RequireOwnership = false)]
		public void AddIngredientServerRpc(ulong netObjectID)
		{
			if (this.TryGet(netObjectID, out var networkObject))
			{
				if (networkObject.TryGetComponent<Ingredient>(out var ingredient))
				{
					if (networkObject.TrySetParent(transform))
					{
						networkObject.transform.localPosition = Vector3.zero;
						networkObject.transform.localRotation = Quaternion.identity;

						_currentProgress.Value = 0.0f;

						if (_cookProgress.Value == Progress.None)
						{
							networkObject.gameObject.transform.localPosition = _progressOffset;
							networkObject.gameObject.transform.localRotation = _ingredientPoint.localRotation;
							networkObject.gameObject.transform.localScale = _ingredientPoint.localScale;
							ingredient.isUIVisable.Value = false;
						}
						else
						{
							networkObject.gameObject.SetActive(false);
						}

						_ingredientObjectIDs.Add(netObjectID);
						_cookProgress.Value = Progress.Progressing;
					}
				}
			}
		}

		#endregion

		#region Cooking
		private bool CanUpdateProgress()
		{
			return _ingredientObjectIDs.Count > 0 && _cookProgress.Value != Progress.Fail;
		}

		[ServerRpc(RequireOwnership = false)]
		public void UpdateProgressServerRpc(float timeDetaTime)
		{
			if (CanUpdateProgress() == false)
				return;

			switch (_cookProgress.Value)
			{
				case Progress.Progressing:
					Debug.Log("Cooking~~~");
					_currentProgress.Value += timeDetaTime;
					if (_currentProgress.Value >= sucessProgress)
						SucessProcessServerRpc();
					break;
				case Progress.Sucess:
					Debug.Log("OverCook~~~");
					_currentProgress.Value += timeDetaTime;
					if (_currentProgress.Value >= failProgress)
						FailProcessServerRpc();
					break;
			}
		}

		[ServerRpc(RequireOwnership = false)]
		private void SucessProcessServerRpc()
		{
			for (int i = 0; i < _ingredientObjectIDs.Count; i++)
			{
				if (this.TryGet(_ingredientObjectIDs[i], out var networkObject))
				{
					if (networkObject.TryGetComponent<Ingredient>(out var ingredient))
					{
						foreach (var recipe in _cookableRecipeList)
						{
							if (ingredient.ingerdientType.Value == recipe.source)
							{
								ingredient.transform.localPosition = _sucessOffset;
								ingredient.ChangeIngredientTypeServerRpc((int)recipe.result);
								onChangeIngredinet?.Invoke(i, recipe.result);
							}
						}
					}
				}
			}

			_cookProgress.Value = Progress.Sucess;
		}

		[ServerRpc(RequireOwnership = false)]
		private void FailProcessServerRpc()
		{
			NetworkObject no;

			while (_ingredientObjectIDs.Count > 1)
			{
				var lastIndex = _ingredientObjectIDs.Count - 1;
				var removeObjectID = _ingredientObjectIDs[lastIndex];

				if (this.TryGet(removeObjectID, out var removeObject))
				{
					removeObject.Despawn();
					_ingredientObjectIDs.RemoveAt(lastIndex);
				}
			}

			if (this.TryGet(_ingredientObjectIDs[0], out no))
			{
				no.GetComponent<Ingredient>().ingerdientType.Value = IngredientType.Trash;
				OnFailClientRpc();
			}

			_cookProgress.Value = Progress.Fail;
		}

		[ClientRpc]
		private void OnFailClientRpc()
		{
			onFail?.Invoke();
		}

		#endregion

		#region Drop Ingredient
		[ServerRpc(RequireOwnership = false)]
		public void DropIngredientToPlateServerRpc(ulong plateID)
		{
			if (_cookProgress.Value != Progress.Sucess)
				return;

			if (this.TryGet(plateID, out var plateObject))
			{
				if (plateObject.TryGetComponent<IAddIngredient>(out var plateAdd))
				{
					//접시에 조리도구를 넣을 수 있는 만큼 조리도구에 있는 재료들을 담는다.
					int i = 0;
					while (_ingredientObjectIDs.Count > 0 && i < _ingredientObjectIDs.Count)
					{
						if (this.TryGet(_ingredientObjectIDs[i], out var ingredientObjct))
						{
							Ingredient ingredient = ingredientObjct.GetComponent<Ingredient>();
							if (plateAdd.CanAdd(ingredient.ingerdientType.Value))
							{
								ingredient.gameObject.SetActive(true);
								ingredient.isUIVisable.Value = true;
								plateAdd.AddIngredientServerRpc(_ingredientObjectIDs[i]);
								_ingredientObjectIDs.RemoveAt(i);
								continue;
							}
						}
						i++;
					}

					//조리도구에 있는 모든 재료가 접시로 이동했을 경우 리셋한다.
					if (_ingredientObjectIDs.Count == 0)
					{
						_cookProgress.Value = Progress.None;
						_currentProgress.Value = 0.0f;
					}
				}
			}
		}


		[ServerRpc(RequireOwnership = false)]
		public void ClearIngredientServerRpc()
		{
			while (_ingredientObjectIDs.Count > 0)
			{
				if (this.TryGet(_ingredientObjectIDs[0], out var ingredientObjct))
				{
					ingredientObjct.Despawn();
					_ingredientObjectIDs.RemoveAt(0);
				}
			}

			//조리도구에 있는 모든 재료가 접시로 이동했을 경우 리셋한다.
			if (_ingredientObjectIDs.Count == 0)
			{
				_cookProgress.Value = Progress.None;
				_currentProgress.Value = 0.0f;
			}

		}
		#endregion
	}
}
