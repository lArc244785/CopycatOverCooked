using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
//using Unity.Netcode.NetworkVariable;

namespace CopycatOverCooked.Orders
{
    public class Order : NetworkBehaviour
    {
        [SerializeField] private Recipe _recipe;
        private Dictionary<RecipeElement, int> _elementTable;

        // NetworkVariable ���
        private NetworkVariable<float> _currentTime = new NetworkVariable<float>();
        private NetworkVariable<float> _maxTime = new NetworkVariable<float>();

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
            while (_currentTime.Value > 0f)
            {
                yield return null;
                _currentTime.Value -= Time.deltaTime;
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

        public IEnumerable<RecipeElement> GetElementTable()
        {
            // _elementTable.Values�� IEnumerable<RecipeElement>�� ��ȯ
            return _elementTable.Values.Cast<RecipeElement>();
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
}