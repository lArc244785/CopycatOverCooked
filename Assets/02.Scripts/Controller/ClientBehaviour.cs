using Unity.Netcode;
using UnityEngine;

public class ClientBehaviour : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;
    [SerializeField] private float _dashSpeed = 0f;
    [SerializeField] private float _throwPower = 0f;
    [SerializeField] private float _itemDetectRange = 0f;
    [SerializeField] private float radius = 0f;
    [SerializeField] private LayerMask _item;
    [SerializeField] private Transform Hand = null;

    private bool _hasItem = false;
    private Vector3 _direction = Vector3.zero;
    private Rigidbody _body = null;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

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

        if(Input.GetKeyDown(KeyCode.F) && DetectItem(out RaycastHit hit) && !_hasItem)
        {
            Debug.Log(hit);
            //GetComponent<NetworkObject>().TrySetParent(_body.GetComponent<NetworkObject>());
            //GetComponent<NetworkObject>().transform.position = Hand.position;
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