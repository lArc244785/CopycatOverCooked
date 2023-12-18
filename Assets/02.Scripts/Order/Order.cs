using System.Collections;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Order : NetworkBehaviour
{
    [SerializeField] private Recipe _recipe;
    private Dictionary<RecipeElement, int> _elementTable;

    [SerializeField] private float _currentTime;
    [SerializeField] private float _maxTime;

    // ���� �� ���� �̺�Ʈ
    public event System.Action OnFailed;
    public event System.Action OnDelivered;

    private void Start()
    {
        // �丮 ��� ���̺� �ʱ�ȭ
        _elementTable = _recipe.Init();

        // �ֹ� Ÿ�̸� ����
        StartCoroutine(OrderTimer());
    }

    private IEnumerator OrderTimer()
    {
        while (_currentTime > 0f)
        {
            yield return null;
            _currentTime -= Time.deltaTime;
        }

        // �ð��� �� �Ǿ� �ֹ� ����
        FailOrder();
    }

    private void Update()
    {
        // �����Ǹ� ���������� Ȯ��
        CheckRecipe();
    }

    private void CheckRecipe()
    {
        foreach (var element in _elementTable)
        {
            // �� ��ᰡ ����� �ִ��� Ȯ��
            if (element.Value <= 0)
            {
                // ��ᰡ �ϳ��� �����ϸ� �ֹ� ����
                FailOrder();
                return;
            }
        }

        // ��� ��ᰡ ����ϸ� �ֹ� ����
        DeliverOrder();
    }

    private void FailOrder()
    {
        OnFailed?.Invoke();
        // �ش� �ֹ� ��ü�� ��� Ŭ���̾�Ʈ���� �ı�
        NetworkObject.Destroy(gameObject);
    }

    private void DeliverOrder()
    {
        OnDelivered?.Invoke();
        // �ش� �ֹ� ��ü�� ��� Ŭ���̾�Ʈ���� �ı�
        NetworkObject.Destroy(gameObject);
    }
}
