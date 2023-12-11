using UnityEngine;
using Photon.Pun;
using System;

public class NetworkedFireExtinguisher : MonoBehaviourPunCallbacks
{
    private bool isHandled = false;
    private bool isEquipped = false;
    private float boxCastDistance;
    private LayerMask fireLayerMask;
    private Vector3 boxCastSize;

    public GameObject cloudParticlePrefab; // 클라우드 파티클 오브젝트를 연결해주기 위한 변수
    public Transform playerHand; // 플레이어의 손 위치를 나타내는 변수

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!isEquipped && Input.GetKeyDown(KeyCode.E))
            {
                photonView.RPC("Grab", RpcTarget.All);
            }

            if (isEquipped && Input.GetMouseButtonDown(0))
            {
                Use();
                photonView.RPC("SyncUse", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void Grab()
    {
        if (!isEquipped)
        {
            transform.SetParent(playerHand); // 소화기를 플레이어의 손에 장착
            transform.localPosition = Vector3.zero; // 플레이어 손 위치에 설정
            isEquipped = true;
        }
        else
        {
            transform.SetParent(null); // 소화기를 플레이어 손에서 해제
            isEquipped = false;
        }
    }

    [PunRPC]
    private void SyncUse()
    {
        Use();
    }

    private void Use()
    {
        if (!isHandled)
        {
            isHandled = true;

            RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxCastSize / 2f, transform.forward, Quaternion.identity, boxCastDistance, fireLayerMask);

            foreach (RaycastHit hit in hits)
            {
                // 클라우드 파티클을 생성하여 소화기에서 발사하는 동작을 만듭니다.
                GameObject cloudParticle = Instantiate(cloudParticlePrefab, transform.position, Quaternion.identity);
                cloudParticle.GetComponent<ParticleSystem>().Play();
                Destroy(cloudParticle, 1.0f); // 원하는 시간 후에 파티클 시스템 제거
            }
        }
    }
}
