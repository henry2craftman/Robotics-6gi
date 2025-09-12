using UnityEngine;

namespace MPS
{
    // ��ǥ : ��ü�� ������ ���� ��ü�� �����ϰ� �����Ǹ� ������ �ٲ��ش�.
    // �Ӽ� : ������ ����(������), ���� ����, ���� ������ ���� renderer, origin ����

    public class Sensor : MonoBehaviour
    {
        public enum SensorType
        {
            ��������,
            �ݼӼ���
        }
        public SensorType type = SensorType.��������;
        public bool isActive = false;
        Renderer renderer;
        Color originColor;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            renderer = GetComponent<Renderer>();
            originColor = renderer.material.color;
        }

        // Update is called once per frame
        void Update()
        {
            if(isActive)
            {
                renderer.material.color = Color.green;
            }
            else
            {
                renderer.material.color = originColor;
            }
        }

        //������ �۵��Ҷ�
        private void OnTriggerEnter(Collider other)
        {
            if(type == SensorType.��������)
            {
                isActive = true;
                renderer.material.color = Color.green;
                print(other.gameObject.name + "������");
            }

            else if(type == SensorType.�ݼӼ���)
            {
                if(other.tag == "Metal")
                {
                    isActive = true;
                    renderer.material.color = Color.green;
                    print(other.gameObject.name + "������");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isActive = false;
            renderer.material.color = originColor;
            print(other.gameObject.name + "����X");
        }
    }
}

