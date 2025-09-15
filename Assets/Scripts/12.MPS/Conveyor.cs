using System.Collections.Generic;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;
// ��ǥ : Dragger���� �۵���Ų��.
// �Ӽ� : Dragger ����Ʈ, ������ ��ȣ, ������ ��ȣ, �ӵ�
namespace MPS
{
    public class Conveyor : MonoBehaviour
    {
        public static Conveyor Instance;
        public Button ConvOnOff;
        [SerializeField] List<Dragger> draggers;
        public bool isConvOnOffSignal = false;
        public bool isCWSignal = false;
        public bool isCCWSignal = false;
        public float speed;
        public Transform startPos;
        public Transform endPos;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (Instance == null)
                Instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnConvOnOffBtnClkEvent()
        {
            isConvOnOffSignal = !isConvOnOffSignal;

            if (isConvOnOffSignal)
            {
                ColorBlock cb = ConvOnOff.colors;
                cb.normalColor = Color.green;
                cb.selectedColor = Color.green;
                ConvOnOff.colors = cb;
            }
            else
            {
                ColorBlock cb = ConvOnOff.colors;
                cb.normalColor = Color.white;
                cb.selectedColor = Color.white;
                ConvOnOff.colors = cb;
            }
        }

        public void OnConvCWBtnClkEvent()
        {
            if (isConvOnOffSignal)
            {
                isCWSignal = !isCWSignal;
            }
            else 
            {
                print("�����̾��� ������ ���� ���ּ���");
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
                print("�����̾��� ������ ���� ���ּ���");
            }
        }
    }
}
