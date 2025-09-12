using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MPS
{
    // ��ǥ : Dragger���� �۵���Ų��.
    // �Ӽ� : Dragger ����Ʈ, ������ ��ȣ, ������ ��ȣ, �ӵ�
    public class Conveyor : MonoBehaviour
    {
        public static Conveyor Instance;

        [SerializeField] List<Dragger> draggers;
        public bool isConvOnOffSignal = false;
        public bool isCWSignal = false; // ������ ��ȣ
        public bool isCCWSignal = false; // ������� ��ȣ
        public float speed;
        public Transform startPos;
        public Transform endPos;


        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnConvCWBtnClkEvent()
        {
            if (isConvOnOffSignal)
            {
                isCWSignal = !isCWSignal;
            }
            else
            {
                print("�����̾� ������ Ȯ�����ּ���");
            }
        }

        public void OnConvCCWBtnClkEvent()
        {
            if (isConvOnOffSignal)
            {
                isCCWSignal = !isCCWSignal;
            }
            else
            {
                print("�����̾� ������ Ȯ�����ּ���");
            }
        }

        public void ConvOnOffBtnClkEvent()
        {
            isConvOnOffSignal = !isConvOnOffSignal;
        }
    }

}
