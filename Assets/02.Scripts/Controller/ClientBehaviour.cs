using CopycatOverCooked;
using CopycatOverCooked.Datas;
using CopycatOverCooked.NetWork.Untesils;
using Unity.Netcode;
using UnityEngine;

public class ClientBehaviour : NetworkBehaviour
{
	[SerializeField] private float _moveSpeed = 0f;
	[SerializeField] private float _dashSpeed = 0f;
	[SerializeField] private float _throwPower = 0f;
	[SerializeField] private float _itemDetectRange = 0f;
	[SerializeField] private float radius = 0f;
	[SerializeField] private LayerMask _interactionObjectLayerMask;
	[SerializeField] private Transform Hand = null;

	private bool _hasItem = false;
	private Vector3 _direction = Vector3.zero;
	private Rigidbody _body = null;

	private NetPickUp _pickUpObject = null;
	private NetUtensillBase _pickUpUtensill;
	private Table _dropTable;
	private Ingredient _pickUpingredient;

	[Header("Detect"), SerializeField] private Vector3 _interactionDetectCenter;
	[SerializeField] private Vector3 _interactionDetectedSize;
	[SerializeField] private float _interactionDistance;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
	}

	private void Awake()
	{
		_body = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!IsOwner)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			_body.AddForce(_direction * _dashSpeed, ForceMode.Impulse);
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			PickUpDropActionServerRpc();
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			UtensillDropIngredientServerRpc();
			ButtonUtensillUpdateProgressServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void PickUpDropActionServerRpc()
	{
		TrashCan trashCan;
		Table dropTable;

		if (_pickUpObject == null)
		{
			if (TryDetectInteraction<NetPickUp>(out var pickUp))
			{
				pickUp.PickUp(transform, Hand.transform.localPosition);
				if (pickUp.TryGetComponent<NetUtensillBase>(out var untensill))
					_pickUpUtensill = untensill;
				else if (pickUp.TryGetComponent<Ingredient>(out var ingredient))
					_pickUpingredient = ingredient;
				_pickUpObject = pickUp;
			}
			if (TryDetectInteraction<IngredientBox>(out var box))
			{
				if (box.TryGetIngredient(out var ingredient))
				{
					_pickUpObject = ingredient.GetComponent<NetPickUp>();
					_pickUpObject.PickUp(transform, Hand.transform.localPosition);
					_pickUpingredient = ingredient;
				}
			}
		}
		else
		{
			//��Ḧ ���� �ִ� ���
			if (_pickUpingredient != null)
			{
				//��������
				if (TryDetectInteraction<NetUtensillBase>(out var utensil))
				{
					if (utensil.TryAddResource(_pickUpingredient))
					{
						_pickUpObject = null;
						_pickUpingredient = null;
					}
					return;
				}
				//����
				else if (TryDetectInteraction<Plate>(out var plate))
				{
					plate.AddIngredient(_pickUpingredient.type.Value);
					_pickUpingredient.GetComponent<NetworkObject>().Despawn();
					_pickUpObject = null;
					_pickUpingredient = null;
					return;
				}
			}
			//���������� ���� �ִ� ���
			else if (_pickUpUtensill != null)
			{
				//����
				if (TryDetectInteraction<Plate>(out var plate))
				{
					plate.AddIngredient(_pickUpUtensill.Spill());
					_pickUpObject = null;
					_pickUpUtensill = null;
					return;
				}

			}

			if (TryDetectInteraction<TrashCan>(out trashCan))
			{
				if(_pickUpObject.TryGetComponent<ISpill>(out var spillable))
				{
					trashCan.TrashUse(spillable);
					return;
				}
				else
				{
					_pickUpingredient.GetComponent<NetworkObject>().Despawn();
					_pickUpingredient = null;
					return;
				}
			}
			
			if(TryDetectInteraction<Table>(out var table))
			{
				table.TryPutObject(_pickUpObject);
				_pickUpObject = null;
				_pickUpUtensill = null;
				_pickUpingredient = null;
			}

		}


	}

	private bool TryDetectInteraction<T>(out T result) where T : MonoBehaviour
	{
		Vector3 start = transform.position + _interactionDetectCenter;
		Vector3 center = start + transform.forward * _interactionDistance;
		result = null;
		var colliders = Physics.OverlapBox(center, _interactionDetectedSize * 0.5f, Quaternion.identity, _interactionObjectLayerMask);
		T hit = null;
		foreach (var collider in colliders)
		{
			hit = collider.GetComponent<T>();
			if (hit != null)
			{
				result = hit;
				return true;
			}

		}
		return false;
	}


	[ServerRpc(RequireOwnership = false)]
	private void UtensillDropIngredientServerRpc()
	{
		if (_pickUpUtensill == null)
			return;

		if (TryDetectInteraction<Plate>(out var plate))
		{
			var spillDatas = _pickUpUtensill.Spill();
			plate.AddIngredient(spillDatas);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void ButtonUtensillUpdateProgressServerRpc()
	{
		if (TryDetectInteraction<NetButtonUtensil>(out var utensill))
		{
			utensill.UpdateProgress();
		}
	}

	private void FixedUpdate()
	{
		if (!IsOwner)
		{
			return;
		}

		move();
	}

	private void move()
	{
		_direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
		_direction.Normalize();

		transform.position += _direction * _moveSpeed * Time.deltaTime;
	}
	private bool DetectItem(out RaycastHit hit)
	{
		if (Physics.Raycast(transform.position + transform.forward * 1.5f + transform.up,
							Vector3.down,
							out hit,
							(transform.position + transform.forward + transform.up).y))
		/*Physics.Raycast(transform.position + transform.forward * 1.5f + transform.up, 
        Vector3.down,
        (transform.position + transform.forward + transform.up).y,
        _item
        )*/
		{
			return true;
		}

		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 start = transform.position + _interactionDetectCenter;
		Vector3 center = start + transform.forward * _interactionDistance;

		Gizmos.color = Color.green;

		Gizmos.DrawLine(start, center);
		Gizmos.DrawWireCube(center, _interactionDetectedSize);
	}
}