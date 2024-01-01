using UnityEngine;

public class TestController : MonoBehaviour
{
    public float speed = 5.0f; // 플레이어의 이동 속도입니다.

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트를 가져옵니다.
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal"); // 수평 방향의 입력을 받습니다. (왼쪽, 오른쪽 방향키)
        float vertical = Input.GetAxis("Vertical"); // 수직 방향의 입력을 받습니다. (위, 아래 방향키)

        Vector3 movement = new Vector3(horizontal, 0, vertical); // 이동 벡터를 생성합니다.

        rb.velocity = movement * speed; // Rigidbody의 속도를 설정하여 플레이어를 이동시킵니다.
    }
}
