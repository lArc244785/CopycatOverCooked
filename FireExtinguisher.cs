using UnityEngine;
using System;

public class FireExtinguisher : MonoBehaviour
{
    private bool isHandled = false;
    private bool isEquipped = false;
    private float boxCastDistance;
    private LayerMask fireLayerMask;
    private Vector3 boxCastSize;

    public ParticleSystem cloudParticlePrefab; // ��ƼŬ �ý��� �������� �������ֱ� ���� ����
    public Transform playerHand; // �÷��̾��� �� ��ġ�� ��Ÿ���� ����

    private void Update()
    {
        if (isEquipped && Input.GetMouseButtonDown(0))
        {
            Use();
        }
    }

    public void Equip()
    {
        transform.SetParent(playerHand); // ��ȭ�⸦ �÷��̾��� �տ� ����
        transform.localPosition = Vector3.zero; // �÷��̾� �� ��ġ�� ����
        isEquipped = true;
    }

    public void Unequip()
    {
        transform.SetParent(null); // ��ȭ�⸦ �÷��̾� �տ��� ����
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
                // ��ƼŬ �ý����� �����Ͽ� ��ȭ�⿡�� �߻��ϴ� ������ ����ϴ�.
                ParticleSystem cloudParticle = Instantiate(cloudParticlePrefab, transform.position, Quaternion.identity);
                cloudParticle.Play();
                Destroy(cloudParticle.gameObject, 1.0f); // ���ϴ� �ð� �Ŀ� ��ƼŬ �ý��� ����
            }
        }
    }
}
