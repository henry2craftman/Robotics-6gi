using System.Collections.Generic;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.UI;
// 목표 : Dragger들을 작동시킨다.
// 속성 : Dragger 리스트, 정방향 신호, 역방향 신호, 속도
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
                print("컨베이어의 전원을 먼저 켜주세요");
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
                print("컨베이어의 전원을 먼저 켜주세요");
            }
        }
    }
}
