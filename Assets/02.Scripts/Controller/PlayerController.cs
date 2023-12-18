//using System.Collections;
//using System.Collections.Generic;
//using System.Globalization;
//using Unity.Netcode;
//using UnityEngine;
//using static UnityEditor.Progress;
//using CopycatOverCooked.Datas;

//public class PlayerController : NetworkBehaviour
//{
//    [SerializeField] private float _moveSpeed = 0f;
//    [SerializeField] private float _dashSpeed = 0f;
//    [SerializeField] private float _throwPower = 0f;
//    [SerializeField] private float _itemDetectRange = 0f;
//    [SerializeField] private float radius = 0f;

//    private bool _ingredientGrabbable = true;
//    private bool _untensilGrabbable = true;
//    private Vector3 _direction = Vector3.zero;
//    private Rigidbody _body = null;

//    //private T item = null;

//    private void Start()
//    {
//        _body = GetComponent<Rigidbody>();
//    }

//    private void Update()
//    {
//        if (!IsOwner)
//        {
//            return;
//        }

//        if (Input.GetKeyDown(KeyCode.LeftShift))
//        {
//            _body.AddForce(_direction * _dashSpeed, ForceMode.Impulse);
//        }

//        if (Input.GetKeyDown(KeyCode.F))
//        {
//            Collider[] hitcollider = Physics.OverlapBox(new Vector3(Hand.position.x, Hand.position.y * 0.5f, Hand.position.z),
//                                                        new Vector3(1f, Hand.position.y, 1f) * 0.5f,
//            quaternion.identity,
//                                                        _item
//                                                        );

//            if (_hasItem.Value == false && hitcollider.Length != 0) //줍기
//            {
//                if (hitcollider[0].TryGetComponent(out IInteract interact))
//                {
//                    Debug.Log(hitcollider[0].GetComponent<NetworkObject>().NetworkObjectId);
//                    InteractionServerRPC(OwnerClientId, hitcollider[0].GetComponent<NetworkObject>().NetworkObjectId);
//                }
//            }

//            if (_hasItem.Value == true)                              //놓기
//            {
//                if (hitcollider.Length == 0 /* || !hitcollider[0].TryGetComponent(UtensilBase)*/)
//                {
//                    PutDown();
//                }

//                else
//                {
//                    return;
//                }
//            }
//        }
//    }

//    private void FixedUpdate()
//    {
//        if (!IsOwner)
//        {
//            return;
//        }

//        move();
//    }

//    private void move()
//    {
//        _direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
//        _direction.Normalize();

//        transform.position += _direction * _moveSpeed * Time.fixedDeltaTime;

//        if (_direction != Vector3.zero)
//        {
//            transform.forward = _direction;
//        }
//    }

//    public void PutDown()
//    {
//        if (_hasItem.Value == false)
//        {
//            return;
//        }

//        _hasItem.Value = false;

//        GetComponent<Ingredient>().GetComponent<NetworkObject>().TryRemoveParent(transform);
//    }

//    private void OnDrawGizmosSelected()
//    {
//        DrawItemDetectGizmos();
//    }

//    private void DrawItemDetectGizmos()
//    {
//        Gizmos.color = Color.cyan;
//        Gizmos.DrawSphere(transform.position + transform.forward * 1.5f, radius);
//        Gizmos.DrawLine(transform.position,
//                        new Vector3(0f, 0f, 0f));

//        Debug.Log(transform.position);
//    }

//    private void DetectObject()
//    {
//        //if(Physics.Raycast(transform.position + transform.forward + transform.up, Vector3.down,                           ))
//    }
//}