using UnityEngine;

// 목표: Led가 PLC로부터 적, 황, 녹 램프신호를 받으면 각 게임오브젝트의 랜더러색상을 변경한다.
// 속성: 각 오브젝트의 Sig들, 각 오브젝트의 랜더러들.
public class TowerLamp : MonoBehaviour
{
    // PLC 신호가 적용될 변수들
    public bool isRedSignal = false;
    public bool isYellowSignal = false;
    public bool isGreenSignal = false;

    public Renderer redLamp;
    public Renderer yellowLamp;
    public Renderer greenLamp;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        redLamp.material.color = Color.black;
        yellowLamp.material.color = Color.black;
        greenLamp.material.color = Color.black;

    }

    // Update is called once per frame
    void Update()
    {
        if (isRedSignal) { redLamp.material.color = Color.red; } else { redLamp.material.color = Color.black; }
        if (isYellowSignal) { yellowLamp.material.color = Color.yellow; } else { yellowLamp.material.color = Color.black; }
        if (isGreenSignal) { greenLamp.material.color = Color.green; } else { greenLamp.material.color = Color.black; }

    }
    public void OnRedLampBtnClkEvent()
    {
        isRedSignal = !isRedSignal;
    }
    public void OnYellowLampBtnClkEvent()
    {
        isYellowSignal = !isYellowSignal;
    }
    public void OnGreenLampBtnClkEvent()
    {
        isGreenSignal = !isGreenSignal;
    }
}
