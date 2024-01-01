using UnityEngine;

public class TestController : MonoBehaviour
{
    public float speed = 5.0f; // �÷��̾��� �̵� �ӵ��Դϴ�.

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ�� �����ɴϴ�.
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // ���� ������ �Է��� �޽��ϴ�. (����, ������ ����Ű)
        float vertical = Input.GetAxis("Vertical"); // ���� ������ �Է��� �޽��ϴ�. (��, �Ʒ� ����Ű)

        Vector3 movement = new Vector3(horizontal, 0, vertical); // �̵� ���͸� �����մϴ�.

        rb.velocity = movement * speed; // Rigidbody�� �ӵ��� �����Ͽ� �÷��̾ �̵���ŵ�ϴ�.
    }
}
