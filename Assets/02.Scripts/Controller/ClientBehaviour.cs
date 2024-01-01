using CopycatOverCooked;
using CopycatOverCooked.Datas;
using CopycatOverCooked.NetWork.Untesils;
using System;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.GamePlay
{
	public class ClientBehaviour : NetworkBehaviour
	{
		[SerializeField] private float _moveSpeed = 0f;
		[SerializeField] private float _dashSpeed = 0f;
		[SerializeField] private float _throwPower = 0f;

		private Vector3 _direction = Vector3.zero;
		private Rigidbody _body = null;
		public Animator animator;

		public event Action<float> onChangeDiractionMagnitude;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			gameObject.name = $"player {OwnerClientId}";
		}

		private void Awake()
		{
			_body = GetComponent<Rigidbody>();
			animator = GetComponent<Animator>();
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
			_direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
			_direction.Normalize();
			onChangeDiractionMagnitude?.Invoke(_direction.sqrMagnitude);

			transform.position += _direction * _moveSpeed * Time.fixedDeltaTime;

			if (_direction != Vector3.zero)
			{
				transform.forward = _direction;
				animator.SetFloat("Walk", 1);
			}
			else
				animator.SetFloat("Walk", 0);

		}

	}
}