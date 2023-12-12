using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0f;
    [SerializeField] private float _dashSpeed = 0f;
    [SerializeField] private float _throwPower = 0f;
    [SerializeField] private float _itemDetectRange = 0f;
    [SerializeField] private float radius = 0f;

    private bool _ingredientGrabbable = true;
    private bool _untensilGrabbable = true;
    private Vector3 _direction = Vector3.zero;
    private Rigidbody _body = null;

    //private T item = null;

    private void Start()
    {
        _body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _body.AddForce(_direction * _dashSpeed, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
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

    private void OnDrawGizmosSelected()
    {
        DrawItemDetectGizmos();
    }

    private void DrawItemDetectGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + transform.forward * 1.5f, radius);
        Gizmos.DrawLine(transform.position,
                        new Vector3(0f, 0f, 0f));

        Debug.Log(transform.position);
    }

    private void DetectObject()
    {
        //if(Physics.Raycast(transform.position + transform.forward + transform.up, Vector3.down,                           ))
    }
}