using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAlwaysCamera : MonoBehaviour
{
    [SerializeField] GameObject setCameraHere;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - setCameraHere.transform.position);
        //���̳ʽ��� �� ������ ���� ���� ��� ī�޶� �����̱� ������ �ſ�ó�� �ݴ�� ���� �� ���� �÷��̾�� ����Ǳ� ����
    }
}