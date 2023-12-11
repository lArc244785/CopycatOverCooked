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
            // ���⿡ ���� Ÿ�� �ִ� ������ ������Ʈ ������ �߰��� �� �ֽ��ϴ�.
        }
    }

    public void Extinguish()
    {
        // ��ȭ�⿡ ���� ȣ��Ǿ� ���� ��ȭ�ϴ� ������ �߰��մϴ�.
        if (isOnFire)
        {
            isOnFire = false;

            // ���� ��ȭ�� �Ŀ� ���ϴ� ������ ������ �� �ֽ��ϴ�.
            Debug.Log("���� ��ȭ�Ǿ����ϴ�.");

            // ���⿡ ���� ��ȭ�� ���� �߰� ������ ������ �� �ֽ��ϴ�.
            // ���� ���, �� ������Ʈ�� ��Ȱ��ȭ�մϴ�.

            gameObject.SetActive(false);
        }
    }

    private void InitializeFire()
    {
        // ���⿡ �� ������Ʈ�� ���۵� �� ����Ǵ� �ʱ�ȭ �ڵ带 �߰��մϴ�.
        // ���� ���, �ʱ� ���� ����, �ʿ��� ������Ʈ �������� ���� ���� �� �ֽ��ϴ�.
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

                // ��ƼŬ �ý����� �����Ͽ� ��ȭ�⿡�� �߻��ϴ� ������ ����ϴ�.
                ParticleSystem cloudParticle = Instantiate(cloudParticlePrefab, transform.position, Quaternion.identity);
                cloudParticle.Play();
                Destroy(cloudParticle.gameObject, 1.0f); // ���ϴ� �ð� �Ŀ� ��ƼŬ �ý��� ����
            }
        }
    }
    */

}
