using CopycatOverCooked;
using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using CopycatOverCooked.Object;
using CopycatOverCooked.Untesil;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FixUtensil : NetworkBehaviour, IInteractable, IAddIngredient
{
	public InteractableType type => InteractableType.FixUtensil;

	private NetworkVariable<ulong> _ingredientObjectID = new NetworkVariable<ulong>(NULL_INGRDIENT);
	private NetworkVariable<Progress> _cookProgress = new NetworkVariable<Progress>(Progress.None);
	[SerializeField] private List<CookRecipe> _cookableRecipeList;
	private const ulong NULL_INGRDIENT = ulong.MaxValue;

	[SerializeField] private Transform _ingredinetPoint;

	[SerializeField] private NetworkVariable<float> _currentProgress = new NetworkVariable<float>(0.0f);
	[SerializeField] private float _updateProgress;
	[SerializeField] private float _surcessProgress;


	public bool CanAdd(IngredientType type)
	{
		if (_ingredientObjectID.Value == NULL_INGRDIENT &&
			   _cookProgress.Value == Progress.None)
		{
			foreach (var recipe in _cookableRecipeList)
			{
				if (type == recipe.source)
					return true;
			}
		}

		return false;
	}


	[ServerRpc(RequireOwnership = false)]
	public void AddIngredientServerRpc(ulong netObjectID)
	{
		if (this.TryGet(netObjectID, out var networkObject))
		{
			if (networkObject.TryGetComponent<Ingredient>(out var inputIngredient))
			{
				if (networkObject.TrySetParent(transform))
				{
					networkObject.transform.localPosition = _ingredinetPoint.localPosition;
					networkObject.transform.localRotation = _ingredinetPoint.localRotation;

					Debug.Log("재료 추가");
					_cookProgress.Value = Progress.Progressing;
					_currentProgress.Value = 0.0f;
					_ingredientObjectID.Value = netObjectID;
				}
			}
		}
	}

	public void BeginInteraction(Interactor interactor)
	{
		switch (_cookProgress.Value)
		{
			case Progress.Progressing:
				if (_cookProgress.Value == Progress.Progressing)
				{
					_currentProgress.Value += _updateProgress;
					if (_currentProgress.Value >= _surcessProgress)
					{
						SucessProcessServerRpc();
					}
				}
				break;

			//플레이어한테 완성된 재료 픽업
			case Progress.Sucess:
				if (this.TryGet(_ingredientObjectID.Value, out var networkObject))
				{
					if (networkObject.TryGetComponent<Pickable>(out var pickable))
					{
						pickable.PickUpServerRpc(interactor.OwnerClientId);
						_ingredientObjectID.Value = NULL_INGRDIENT;
						_cookProgress.Value = Progress.None;
					}
				}
				break;
		}
	}

	public void EndInteraction(Interactor interactor)
	{
		if (this.TryGet(interactor.currentInteractableNetworkObjectID.Value, out var pickObjcet))
		{
			if (pickObjcet.TryGetComponent<IInteractable>(out var other))
			{
				switch (other.type)
				{
					case InteractableType.Plate:
						DropIngredientToPlateServerRpc(pickObjcet.NetworkObjectId);
						break;
					case InteractableType.Ingredient:
						Ingredient otherIngredient = (Ingredient)other;
						if (CanAdd(otherIngredient.ingerdientType.Value))
						{
							AddIngredientServerRpc(otherIngredient.NetworkObjectId);
						}
						break;
				}
			}

		}

	}

	[ServerRpc(RequireOwnership = false)]
	private void SucessProcessServerRpc()
	{
		if (this.TryGet(_ingredientObjectID.Value, out var networkObject))
		{
			if (networkObject.TryGetComponent<Ingredient>(out var ingredient))
			{
				//재료를 완료 재료 타입으로 변경
				foreach (var recipe in _cookableRecipeList)
				{
					if (ingredient.ingerdientType.Value == recipe.source)
						ingredient.ingerdientType.Value = recipe.result;
				}
			}
		}

		_cookProgress.Value = Progress.Sucess;
	}


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
				if (this.TryGet(_ingredientObjectID.Value, out var ingredientObjct))
				{
					Ingredient ingredient = ingredientObjct.GetComponent<Ingredient>();
					if (plateAdd.CanAdd(ingredient.ingerdientType.Value))
					{
						plateAdd.AddIngredientServerRpc(_ingredientObjectID.Value);
						_ingredientObjectID.Value = NULL_INGRDIENT;
						_cookProgress.Value = Progress.None;
					}
				}
			}
		}
	}

	#endregion
}
