using System;
using System.Collections;
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

		private bool isControl => GameManager.instance.state.Value == GameFlow.Play;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			gameObject.name = $"player {OwnerClientId}";

			if (IsOwner)
			{
				_body.isKinematic = true;
				StartCoroutine(C_WaitPlayer());
			}
		}

		private IEnumerator C_WaitPlayer()
		{
			yield return new WaitUntil(() => GameManager.instance.state.Value == GameFlow.Load);
			yield return new WaitUntil(() => StageManager.instance != null);
			yield return new WaitUntil(() => StageManager.instance.currentStep.Value >= StageManager.Step.WaitUntilAllPlayersAreReady);
			transform.position = StageManager.instance.GetStartPoint((int)OwnerClientId);
			_body.isKinematic = false;
		}

		private void Awake()
		{
			_body = GetComponent<Rigidbody>();
			animator = GetComponent<Animator>();
		}

		private void Update()
		{
			if (!IsOwner || !isControl)
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
			if (!IsOwner || !isControl)
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
				animator?.SetFloat("Walk", 1);
			}
			else
			{
				animator?.SetFloat("Walk", 0);
			}

		}

	}
}