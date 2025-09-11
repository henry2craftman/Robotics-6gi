using UnityEngine;

namespace Conveyor
{
    public class Sensor : MonoBehaviour
    {
        public bool isSensing = false;

        private void OnTriggerEnter(Collider other)
        {
            print(other.name);
            isSensing = true;

            Conveyor.Instance.InvokeMovePusher(this); // �����̾ � �������� �˼� �ֵ��� ���� ����
        }

        private void OnTriggerExit(Collider other)
        {
            isSensing = false;
        }
    }

}
