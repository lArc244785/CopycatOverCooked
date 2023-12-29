using UnityEngine;

public class Fire : MonoBehaviour
{
    private bool isOnFire = true;

    private void Start()
    {
        InitializeFire();
    }

    private void Update()
    {
        if (isOnFire)
        {
            // 여기에 불이 타고 있는 동안의 업데이트 로직을 추가할 수 있습니다.
        }
    }

    public void Extinguish()
    {
        // 소화기에 의해 호출되어 불을 소화하는 로직을 추가합니다.
        if (isOnFire)
        {
            isOnFire = false;

            // 불을 소화한 후에 원하는 동작을 수행할 수 있습니다.
            Debug.Log("불이 소화되었습니다.");

            // 여기에 불을 소화한 후의 추가 동작을 수행할 수 있습니다.
            // 예를 들어, 불 오브젝트를 비활성화합니다.

            gameObject.SetActive(false);
        }
    }

    private void InitializeFire()
    {
        // 여기에 불 오브젝트가 시작될 때 실행되는 초기화 코드를 추가합니다.
        // 예를 들어, 초기 상태 설정, 필요한 컴포넌트 가져오기 등이 있을 수 있습니다.
    }

    /*
    public void Use()
    {
        if (!isHandled)
        {
            isHandled = true;
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxCastSize / 2f, transform.forward, Quaternion.identity, boxCastDistance, fireLayerMask);

            foreach (RaycastHit hit in hits)
            {
                Fire fireComponent = hit.collider.GetComponent<Fire>();
                if (fireComponent != null)
                {
                    fireComponent.Extinguish();
                }

                // 파티클 시스템을 생성하여 소화기에서 발사하는 동작을 만듭니다.
                ParticleSystem cloudParticle = Instantiate(cloudParticlePrefab, transform.position, Quaternion.identity);
                cloudParticle.Play();
                Destroy(cloudParticle.gameObject, 1.0f); // 원하는 시간 후에 파티클 시스템 제거
            }
        }
    }
    */

}
