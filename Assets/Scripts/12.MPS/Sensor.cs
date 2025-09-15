using UnityEngine;

namespace MPS
{
    // ��ǥ : ��ü�� ������ ���� ��ü�� �����ϰ� �����Ǹ� ������ �ٲ��ش�
    // �Ӽ� : ������ ����(������), ��������, ���󺯰��� ���� ������, �������� ����

    public class Sensor : MonoBehaviour
    {
        public enum SensorType
        {
            ��������,
            �ݼӰ�������
        }
        public SensorType type = SensorType.��������;
        public bool isActive = false;

        new Renderer renderer;
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
            if (isActive)
            {
                renderer.material.color = Color.green;
            }
            else
            {
                renderer.material.color = originColor;
            }

        }
        // ������ �۵��� ��
        private void OnTriggerEnter(Collider other)
        {
            if (type == SensorType.��������)
            {
                    isActive = true;
                    print(other.gameObject.name + "������");
             

            }
            else if (type == SensorType.�ݼӰ�������)
            {
                
                    if (other.tag == "Metal")
                    {
                        isActive = true;
                        print(other.gameObject.name + "������");
                    }
                }
            }
                       
        private void OnTriggerExit(Collider other)
        {
            isActive = false;
            print(other.gameObject.name + "������");
        }

        
    }
}

