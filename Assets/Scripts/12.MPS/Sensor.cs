using UnityEngine;

namespace MPS
{
    // 목표 : 물체의 종류에 따라 물체를 감지하고 감지되면 색상을 바꿔준다.
    // 속성 : 센서의 종류(열거형), 센서 상태, 색상 변경을 위한 renderer, origin 색상

    public class Sensor : MonoBehaviour
    {
        public enum SensorType
        {
            근접센서,
            금속센서
        }
        public SensorType type = SensorType.근접센서;
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

        //센서가 작동할때
        private void OnTriggerEnter(Collider other)
        {
            if(type == SensorType.근접센서)
            {
                isActive = true;
                renderer.material.color = Color.green;
                print(other.gameObject.name + "감지중");
            }

            else if(type == SensorType.금속센서)
            {
                if(other.tag == "Metal")
                {
                    isActive = true;
                    renderer.material.color = Color.green;
                    print(other.gameObject.name + "감지중");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isActive = false;
            renderer.material.color = originColor;
            print(other.gameObject.name + "감지X");
        }
    }
}

