using UnityEngine;

namespace MPS
{
    // 목표 : 물체의 종류에 따라 물체를 감지하고 감지되면 색상을 바꿔준다
    // 속성 : 센서의 종류(열거형), 센서상태, 색상변경을 위한 랜더러, 원래색상 저장

    public class Sensor : MonoBehaviour
    {
        public enum SensorType
        {
            근접센서,
            금속감지센서
        }
        public SensorType type = SensorType.근접센서;
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
        // 센서가 작동할 때
        private void OnTriggerEnter(Collider other)
        {
            if (type == SensorType.근접센서)
            {
                    isActive = true;
                    print(other.gameObject.name + "감지중");
             

            }
            else if (type == SensorType.금속감지센서)
            {
                
                    if (other.tag == "Metal")
                    {
                        isActive = true;
                        print(other.gameObject.name + "감지중");
                    }
                }
            }
                       
        private void OnTriggerExit(Collider other)
        {
            isActive = false;
            print(other.gameObject.name + "감지끝");
        }

        
    }
}

