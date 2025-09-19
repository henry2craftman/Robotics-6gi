using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MPS
{
    // 목표: Dragger들을 작동시킨다.
    // 속성: Dragger 리스트, 정방향신호, 역방향신호, 속도
    public class Conveyor : MonoBehaviour
    {
        public static Conveyor Instance;

        [SerializeField] List<Dragger> draggers;
        public bool isConvOnOffSignal = false;
        public bool isCWSignal = false;  // 정방향 신호
        public bool isCCWSignal = false; // 역방향 신호
        public float speed;
        public Transform startPos;
        public Transform endPos;

        // UI
        public Button convOnOffBtn;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
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
                ColorBlock cb = convOnOffBtn.colors;
                cb.normalColor = Color.green;
                cb.selectedColor = Color.green;
                convOnOffBtn.colors = cb;
            }
            else
            {
                ColorBlock cb = convOnOffBtn.colors;
                cb.normalColor = Color.red;
                cb.selectedColor = Color.red;
                convOnOffBtn.colors = cb;
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
                print("컨베이어의 전원을 먼저 켜주세요.");
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
                print("컨베이어의 전원을 먼저 켜주세요.");
            }
        }

    }
}
