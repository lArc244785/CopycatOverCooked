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
        // todo -> clientId 에 해당하는 플레이어 찾아서 해당 NetworkObject에 이 소화기를 종속시키기.
        // todo -> 플레이어 컨트롤러가 상호작용 객체 참조에 이 소화기를 가지도록 참조 대입해줘야함.
        isPickedUp.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void PutDownServerRpc(ulong clientID)
    {
        // todo -> 소화기 \NetworkObject 종속 풀기
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
            Debug.Log("소화기 발사!");
        }
    }
}
