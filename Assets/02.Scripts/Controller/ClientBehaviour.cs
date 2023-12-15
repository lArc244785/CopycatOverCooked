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

    private bool _hasItem = false;
    private Vector3 _direction = Vector3.zero;
    private Rigidbody _body = null;

    private NetPickUp _pickUpObject;
    private NetUtensillBase _pickUpUtensill;
    private Table _dropTable;

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

        if(Input.GetKeyDown(KeyCode.F))
        {
            PickUpDropActionServerRpc();
		}
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpDropActionServerRpc()
    {
        if (_pickUpObject == null)
        {
            _pickUpObject = DetectInteraction<NetPickUp>();
            _pickUpObject?.PickUp(transform, Hand.transform.localPosition);
			_pickUpUtensill = _pickUpObject.GetComponent<NetUtensillBase>();

		}
        else
        {
            var dropTable = DetectInteraction<Table>();
            if (dropTable == null)
                return;

            dropTable.PutObject(_pickUpObject);
            _pickUpObject = null;
        }
    }

    private T DetectInteraction<T>() where T : MonoBehaviour
    {
		Vector3 start = transform.position + _interactionDetectCenter;
		Vector3 center = start + transform.forward * _interactionDistance;

        var colliders = Physics.OverlapBox(center, _interactionDetectedSize * 0.5f, Quaternion.identity, _interactionObjectLayerMask);
        T hit = null;
        foreach(var collider in colliders)
        {
            hit = collider.GetComponent<T>();
            if (hit != null)
                return hit;
        }
        return null;
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