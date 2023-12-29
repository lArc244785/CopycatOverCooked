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

	private Vector3 _direction = Vector3.zero;
	private Rigidbody _body = null;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		gameObject.name = $"player {OwnerClientId}";
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
		_direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		_direction.Normalize();

		transform.position += _direction * _moveSpeed * Time.fixedDeltaTime;

		if (_direction != Vector3.zero)
		{
			transform.forward = _direction;
		}
	}
}