using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class AnimationManager : NetworkAnimator
{
    //[SerializeField] private ClientBehaviour _playerMove;

    private void Awake()
    {
        if (IsOwner)
        {
            //_playerMove.onChangeDiractionMagnitude += OnUpdateMove;
        }
    }

    private void OnUpdateMove(float magnitude)
    {
        magnitude = Mathf.Clamp01(magnitude);
        

        //todo - animator.SetFloat("Move", magnitude)
    }

}
