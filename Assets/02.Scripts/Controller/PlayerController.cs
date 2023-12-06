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

    private float _tempSpeed = 0f;
    private bool _ingredientGrabbable = true;
    private bool _untensilGrabbable = true;

    /*
    

    */

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //_moveSpeed =
        }
    }

    private void FixedUpdate()
    {
        move();
    }

    private void OnDrawGizmosSelected()
    {
        DrawItemDetectGizmos();
    }

    private void DrawItemDetectGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.forward * _itemDetectRange, radius);
    }

    private void move()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")) * _moveSpeed * Time.deltaTime;
        transform.forward = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }
}