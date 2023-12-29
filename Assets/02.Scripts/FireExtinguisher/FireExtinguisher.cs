using UnityEngine;
using Unity.Netcode;

public class FireExtinguisher : NetworkBehaviour
{
    NetworkVariable<bool> isPickedUp = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private bool isHandled = false;
    public GameObject extinguishedObjectPrefab;
    public KeyCode shootKey = KeyCode.A;

    private void Update()
    {
        if (!IsOwner)
            return;
    }

    public bool TryPickUp(ulong clientID)
    {
        if (isPickedUp.Value)
            return false;

        PickUpServerRpc(clientID);
        return true;
    }

    public void Interaction()
    {
        // 
    }


    [ServerRpc(RequireOwnership = false)]
    private void PickUpServerRpc(ulong clientID)
    {
        // todo -> clientId �� �ش��ϴ� �÷��̾� ã�Ƽ� �ش� NetworkObject�� �� ��ȭ�⸦ ���ӽ�Ű��.
        // todo -> �÷��̾� ��Ʈ�ѷ��� ��ȣ�ۿ� ��ü ������ �� ��ȭ�⸦ �������� ���� �����������.
        isPickedUp.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void PutDownServerRpc(ulong clientID)
    {
        // todo -> ��ȭ�� \NetworkObject ���� Ǯ��
        isPickedUp.Value = false;
    }


    [ServerRpc(RequireOwnership = false)]
    private void UseServerRpc()
    {
        if (!isHandled)
        {
            isHandled = true;
            RpcSpawnExtinguishedObjectClientRpc();
        }
    }

    [ClientRpc]
    private void RpcSpawnExtinguishedObjectClientRpc()
    {
        if (!isHandled)
        {
            GameObject extinguishedObject = Instantiate(extinguishedObjectPrefab, transform.position, Quaternion.identity);
            ShootObject(extinguishedObject);
        }
    }

    private void ShootObject(GameObject objToShoot)
    {
        Rigidbody rb = objToShoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
            Debug.Log("��ȭ�� �߻�!");
        }
    }
}
