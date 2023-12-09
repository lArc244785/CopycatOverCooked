using UnityEngine;
using System;

public class FireExtinguisher : MonoBehaviour
{
    private bool isHandled = false;
    private bool isEquipped = false;
    private float boxCastDistance;
    private LayerMask fireLayerMask;
    private Vector3 boxCastSize;

    public ParticleSystem cloudParticlePrefab; // 파티클 시스템 프리팹을 연결해주기 위한 변수
    public Transform playerHand; // 플레이어의 손 위치를 나타내는 변수

    private void Update()
    {
        if (isEquipped && Input.GetMouseButtonDown(0))
        {
            Use();
        }
    }

    public void Equip()
    {
        transform.SetParent(playerHand); // 소화기를 플레이어의 손에 장착
        transform.localPosition = Vector3.zero; // 플레이어 손 위치에 설정
        isEquipped = true;
    }

    public void Unequip()
    {
        transform.SetParent(null); // 소화기를 플레이어 손에서 해제
        isEquipped = false;
    }

    public void Use()
    {
        if (!isHandled)
        {
            isHandled = true;
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxCastSize / 2f, transform.forward, Quaternion.identity, boxCastDistance, fireLayerMask);

            foreach (RaycastHit hit in hits)
            {
                // 파티클 시스템을 생성하여 소화기에서 발사하는 동작을 만듭니다.
                ParticleSystem cloudParticle = Instantiate(cloudParticlePrefab, transform.position, Quaternion.identity);
                cloudParticle.Play();
                Destroy(cloudParticle.gameObject, 1.0f); // 원하는 시간 후에 파티클 시스템 제거
            }
        }
    }
}
