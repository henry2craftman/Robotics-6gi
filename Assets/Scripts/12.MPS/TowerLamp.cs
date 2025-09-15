using UnityEngine;

// ��ǥ: Led�� PLC�κ��� ��, Ȳ, �� ������ȣ�� ������ �� ���ӿ�����Ʈ�� ������������ �����Ѵ�.
// �Ӽ�: �� ������Ʈ�� Sig��, �� ������Ʈ�� ��������.
public class TowerLamp : MonoBehaviour
{
    // PLC ��ȣ�� ����� ������
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
