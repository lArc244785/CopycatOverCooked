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
        transform.rotation = Quaternion.Euler(60,0,0);
        //마이너스를 한 이유는 하지 않을 경우 카메라 기준이기 때문에 거울처럼 반대로 찍힌 것 마냥 플레이어에게 노출되기 때문
    }
}
