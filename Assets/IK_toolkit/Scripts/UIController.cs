using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Serializable]
    public class TeachData
    {
        public int stepNum;
        public Vector3 pos; // ��ġ����
        public Vector3 rot; // ��������
        public float duration;
        public bool isGripperOn;
    }

    public IK_toolkit ikToolkit;
    
    public TMP_InputField xInput;
    public TMP_InputField yInput;
    public TMP_InputField zInput;
    public TMP_InputField durationInput;
    public Toggle gripperToggle;
    public string teachDataPath;
    public float multiplier = 0.01f;
    public float rotMultiplier = 0.1f;
    public float reach = 2;
    public List<TeachData> teachDatas = new List<TeachData>();

    bool isXPlusBtnClicking;
    bool isXMinusBtnClicking;
    bool isYPlusBtnClicking;
    bool isYMinusBtnClicking;
    bool isZPlusBtnClicking;
    bool isZMinusBtnClicking;
    float x, y, z;
    float xRot, yRot, zRot;
    int stepCnt;
    Vector3 currentPos;
    private bool isXRotPlusClicking;
    private bool isXRotMinusClicking;
    private bool isYRotPlusClicking;
    private bool isYRotMinusClicking;
    private bool isZRotPlusClicking;
    private bool isZRotMinusClicking;

    private void Start()
    {
        x = ikToolkit.ik.position.x;
        y = ikToolkit.ik.position.y;
        z = ikToolkit.ik.position.z;

        xRot = ikToolkit.ik.rotation.eulerAngles.x;
        yRot = ikToolkit.ik.rotation.eulerAngles.y;
        zRot = ikToolkit.ik.rotation.eulerAngles.z;

        teachDataPath = Application.persistentDataPath + "/teachingData.txt";

        durationInput.text = "2";

        InitializeData();
    }

    // teachData.txt���� ������ �о�� ��, teachDatas List�� �ֱ�
    private void InitializeData()
    {
        if(!File.Exists(teachDataPath))
        {
            File.Create(teachDataPath);
        }
        else
        {
            using(FileStream fs = new FileStream(teachDataPath, FileMode.Open))
            {
                using(StreamReader sr = new StreamReader(fs))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            TeachData data = new TeachData();

                            // 0,(-0.41, 0.71, 1.08),(1, 3, 5),0.5,True
                            char stepNum = line[0];

                            // Position Parsing -> -0.41, 0.71, 1.08
                            int indexOpenBraket = line.IndexOf('(');    // 2
                            int indexCloseBraket = line.IndexOf(')');   // 20
                            // -0.41, 0.71, 1.08
                            string position = line.Substring(indexOpenBraket + 1, indexCloseBraket - indexOpenBraket - 1);

                            // Rotation Parsing -> 1, 3, 5
                            int indexOpenBraket2 = line.IndexOf('(', line.IndexOf('(') + 1);
                            int indexCloseBraket2 = line.IndexOf(')', line.IndexOf(')') + 1);
                            string rotation = line.Substring(indexOpenBraket2 + 1, indexCloseBraket2 - indexOpenBraket2 - 1);

                            string leftOver = line.Remove(0, indexCloseBraket2 + 2);  // 0.5,True
                            string[] leftOvers = leftOver.Split(',');
                            string duration = leftOvers[0];              // 0.5
                            string isGripperOn = leftOvers[1];           // True

                            data.stepNum = Convert.ToInt32(stepNum) - '0';
                            string[] pos = position.Split(',');
                            string[] rot = rotation.Split(',');
                            data.pos = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                            data.rot = new Vector3(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]));
                            data.duration = float.Parse(duration);
                            data.isGripperOn = Convert.ToBoolean(isGripperOn);

                            teachDatas.Add(data);
                        }
                        catch(Exception e)
                        {
                            Debug.LogWarning("�ùٸ� �����Ͱ� �ƴմϴ�. �����͸� Ȯ�� �� �ٽ� �õ��� �ּ���.");

                            return;
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AutomationController.instance.isRobotRunning)
            return;

        bool isMovable = CanMove();

        if (!isMovable)
            return;

        UpdatePosValues();

        UpdateRotValues();

        UpdateEndEffector(); // position, rotation updata
    }

    private void UpdatePosValues()
    {
        if (isXPlusBtnClicking)
        {
            x += multiplier;
        }

        if (isXMinusBtnClicking)
        {
            x -= multiplier;
        }

        if (isYPlusBtnClicking)
        {
            y += multiplier;
        }

        if (isYMinusBtnClicking)
        {
            y -= multiplier;
        }

        if (isZPlusBtnClicking)
        {
            z += multiplier;
        }

        if (isZMinusBtnClicking)
        {
            z -= multiplier;
        }
    }

    private void UpdateRotValues()
    {
        if (isXRotPlusClicking)
        {
            xRot += rotMultiplier;
        }

        if (isXRotMinusClicking)
        {
            xRot -= rotMultiplier;
        }

        if (isYRotPlusClicking)
        {
            yRot += rotMultiplier;
        }

        if (isYRotMinusClicking)
        {
            yRot -= rotMultiplier;
        }

        if (isZRotPlusClicking)
        {
            zRot += rotMultiplier;
        }

        if (isZRotMinusClicking)
        {
            zRot -= rotMultiplier;
        }
    }

    private void UpdateEndEffector()
    {
        xInput.text = x.ToString();
        yInput.text = y.ToString();
        zInput.text = z.ToString();

        // ikToolkit�� Base���� end-effector�� �Ÿ���
        // Ư�� �Ÿ� �̻��� �Ǹ� �Ʒ� �ڵ� ���� X

        ikToolkit.ik.position = new Vector3(x, y, z);
        ikToolkit.ik.rotation = Quaternion.Euler(xRot, yRot, zRot);
    }

    private bool CanMove()
    {
        Vector3 dir = ikToolkit.ik.position - ikToolkit.robot[0].position;
        float distance = dir.magnitude;

        if (distance >= reach)
        {
            Debug.LogWarning("�Է°��� ��ġ�� �ʰ��Ͽ����ϴ�.");

            ikToolkit.ik.position = currentPos;
            return false;
        }
        else
        {
            currentPos = ikToolkit.ik.position;
            return true;
        }
    }

    // teach��ư�� ������ �κ��� End-effector�� ��ġ��
    // List�� �����ϰ�, ���ÿ� text ���� teachingData.txt�� ����
    // position,duration,isSuctionOn
    // step1,3,5,6,3.5,true
    // step2,3,5,6,3.5,true
    // step3,3,5,6,3.5,true

    // ���۹�ư�� ������ AutomationController�� ������ ����
    // Ư�� Vector3�� duration ���� �̵�, ȸ��
    // 1. MoveRobotTo�� Vector3 �������� �����ε�
    // 2. AutomationController�� StartRobot �Լ� ����
    // 2-1. StartRobot �Լ� ���� Coroutine ����!
    //    -> ������ 1�� ����
    public void OnStartBtnClkEvent()
    {
        AutomationController.instance.StartSequence(teachDatas);
    }

    public void OnCycleBtnClkEvent()
    {

    }

    public void OnStopBtnClkEvent()
    {

    }

    public void OnTeachBtnClkEvent()
    {
        TeachData teachData = new TeachData();
        teachData.stepNum = stepCnt++;

        bool isFloat = float.TryParse(durationInput.text, out teachData.duration);
        if (!isFloat)
            teachData.duration = 0;

        teachData.isGripperOn = gripperToggle.isOn;
        teachData.pos = ikToolkit.ik.position;
        teachData.rot = ikToolkit.ik.eulerAngles;

        teachDatas.Add(teachData);

        AddLine(teachData);
    }

    private void AddLine(TeachData teachData)
    {
        if(File.Exists(teachDataPath))
        {
            using(FileStream fs = new FileStream(teachDataPath, FileMode.Append))
            {
                using(StreamWriter sw = new StreamWriter(fs))
                {
                    string data = $"{teachData.stepNum},{teachData.pos},{teachData.rot},{teachData.duration},{teachData.isGripperOn}";
                    sw.WriteLine(data);
                    Debug.Log($"������ �߰�: {data}");
                }
            }
        }
        else
        {
            using (FileStream fs = new FileStream(teachDataPath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string data = $"{teachData.stepNum},{teachData.pos},{teachData.duration},{teachData.isGripperOn}";
                    sw.WriteLine(data);
                    Debug.Log($"������ �߰�: {data}");
                }
            }
        }
    }

    public void OnDeleteBtnClkEvent()
    {
        // 1. TeachData ����Ʈ �ʱ�ȭ
        stepCnt = 0;
        teachDatas.Clear();

        // 2. Txt ���� ���� �����
        if(File.Exists(teachDataPath))
        {
            File.WriteAllText(teachDataPath, string.Empty);

            Debug.Log("Data�� ��� �������ϴ�.");
        }
    }

    // X
    public void OnXPlusBtnDownEvent()
    {
        isXPlusBtnClicking = true;
    }
    public void OnXPlusBtnUpEvent()
    {
        isXPlusBtnClicking = false;
    }
    public void OnXMinusBtnDownEvent()
    {
        isXMinusBtnClicking = true;
    }
    public void OnXMinusBtnUpEvent()
    {
        isXMinusBtnClicking = false;
    }
    // Y
    public void OnYPlusBtnDownEvent()
    {
        isYPlusBtnClicking = true;
    }
    public void OnYPlusBtnUpEvent()
    {
        isYPlusBtnClicking = false;
    }
    public void OnYMinusBtnDownEvent()
    {
        isYMinusBtnClicking = true;
    }
    public void OnYMinusBtnUpEvent()
    {
        isYMinusBtnClicking = false;
    }
    // Z
    public void OnZPlusBtnDownEvent()
    {
        isZPlusBtnClicking = true;
    }
    public void OnZPlusBtnUpEvent()
    {
        isZPlusBtnClicking = false;
    }
    public void OnZMinusBtnDownEvent()
    {
        isZMinusBtnClicking = true;
    }
    public void OnZMinusBtnUpEvent()
    {
        isZMinusBtnClicking = false;
    }
    // xRot
    public void OnXRotPlusBtnDownEvent()
    {
        isXRotPlusClicking = true;
    }
    public void OnXRotPlusBtnUpEvent()
    {
        isXRotPlusClicking = false;
    }
    public void OnXRotMinusBtnDownEvent()
    {
        isXRotMinusClicking = true;
    }
    public void OnXRotMinusBtnUpEvent()
    {
        isXRotMinusClicking = false;
    }
    // yRot
    public void OnYRotPlusBtnDownEvent()
    {
        isYRotPlusClicking = true;
    }
    public void OnYRotPlusBtnUpEvent()
    {
        isYRotPlusClicking = false;
    }
    public void OnYRotMinusBtnDownEvent()
    {
        isYRotMinusClicking = true;
    }
    public void OnYRotMinusBtnUpEvent()
    {
        isYRotMinusClicking = false;
    }
    // zRot
    public void OnZRotPlusBtnEvent(bool click)
    {
        //click = !click;
        isZRotPlusClicking = click;
    }
    public void OnZRotMinusBtnEvent(bool click)
    {
        isZRotMinusClicking = click;
    }
}
