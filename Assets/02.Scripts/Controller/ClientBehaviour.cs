using CopycatOverCooked.NetWork.Untesils;
using System.Runtime.InteropServices.WindowsRuntime;
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

    private Vector3 _direction = Vector3.zero;
    private Rigidbody _body = null;

    [Header("Detect"), SerializeField] private Vector3 _interactionDetectCenter;
    [SerializeField]private Vector3 _interactionDetectedSize;
    [SerializeField]private float _interactionDistance;
    
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

}