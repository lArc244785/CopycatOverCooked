using CopycatOverCooked;
using CopycatOverCooked.Datas;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Serve : NetworkBehaviour
{
    [SerializeField] private Transform _plateSpawnPoint;
    [SerializeField] private Transform _putPoint;
    [SerializeField] private LayerMask _putLayerMask;

    public NetworkVariable<bool> canPutObject = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetPickUp _dropObject;
    private float _eatingTime = 3f;
    private float _progressTime = 0f;
        
    //Order참조

    public bool ServeFood(NetPickUp dropObject)              //Table의 TryPutObject 가져와 사용
    {
        if (IsServer == false)
            return false;
        if (canPutObject.Value == false)
            return false;

        if ((_putLayerMask & 1 << dropObject.gameObject.layer) > 0)
        {
            IngredientType[] food = dropObject.GetComponent<Plate>().Spill();

            if (food.Length != 0 /*&& Order참조*/)
            {
                dropObject.transform.parent = null;
                dropObject.transform.position = _putPoint.position;
                canPutObject.Value = false;
                _dropObject = dropObject;
                _dropObject.onPickUp += DropObject;

                deactiveObjectClientRpc();

                return true;
            }
        }

        return false;
    }

    private void DropObject()
    {
        canPutObject.Value = true;
        _dropObject.onPickUp -= DropObject;
    }

    [ClientRpc]
    public void deactiveObjectClientRpc()
    {
        _dropObject.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void activeObjectClientRpc() 
    {
        _dropObject.gameObject.SetActive(true);
    }
}