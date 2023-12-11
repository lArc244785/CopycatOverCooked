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

    public GameObject cloudParticlePrefab; // Ŭ���� ��ƼŬ ������Ʈ�� �������ֱ� ���� ����
    public Transform playerHand; // �÷��̾��� �� ��ġ�� ��Ÿ���� ����

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
            transform.SetParent(playerHand); // ��ȭ�⸦ �÷��̾��� �տ� ����
            transform.localPosition = Vector3.zero; // �÷��̾� �� ��ġ�� ����
            isEquipped = true;
        }
        else
        {
            transform.SetParent(null); // ��ȭ�⸦ �÷��̾� �տ��� ����
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
                // Ŭ���� ��ƼŬ�� �����Ͽ� ��ȭ�⿡�� �߻��ϴ� ������ ����ϴ�.
                GameObject cloudParticle = Instantiate(cloudParticlePrefab, transform.position, Quaternion.identity);
                cloudParticle.GetComponent<ParticleSystem>().Play();
                Destroy(cloudParticle, 1.0f); // ���ϴ� �ð� �Ŀ� ��ƼŬ �ý��� ����
            }
        }
    }
}
