using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static RobotUIManager;

// 목표: UI에 연결된 Position, Rotation 값을 바꿔서 로봇에 적용한다.
// 속성: OriginEndPos, OriginEndRot, 로봇전원상태, 석션상태토글
// Step 정보 저장을 위한 기능(Step번호, 포지션, 로테이션, Duration, isSuctionOn)
public class RobotUIManager : MonoBehaviour
{
    [Serializable]
    public class RobotSequence
    {
        public List<List<Step>> sequenceList = new List<List<Step>>();

        public void AddSequence(List<Step> steps)
        {
            sequenceList.Add(steps);
        }
    }
    public RobotSequence robotSequence = new RobotSequence();

    [Serializable]
    public struct Step
    {
        public int stepNum;
        public Vector3 position;
        public Quaternion rotation;
        public float duration;
        public bool isSuctionOn;
    }
    public int repeatCount = 0;
    public List<Step> steps = new List<Step>();


    public bool isRobotOn = false;    // Power
    public bool isStarted = false;     // Start 버튼 클릭 여부
    public bool isSequenceOn = false; // Cycle 작동 여부
    public bool isEmergency = false;  // E-Stop 버튼 클릭 여부
    public bool isSuctionOn = false;  // 현재 스탭의 suction 상태

    // EndEffector의 초기 Pos, Rot
    public Vector3 OriginEndPos;
    public Quaternion OriginEndRot;

    public TMP_InputField xPosInput;
    public TMP_InputField yPosInput;
    public TMP_InputField zPosInput;
    public TMP_InputField xRotInput;
    public TMP_InputField yRotInput;
    public TMP_InputField zRotInput;
    public Toggle suctionToggle;
    public Toggle teachByToggle;

    float x, y, z;
    float xRot, yRot, zRot;
    public float multiplier = 0.01f;
    public float rotMultiplier = 0.1f;
    bool isXPlusBtnDowning = false;
    bool isYPlusBtnDowning = false;
    bool isZPlusBtnDowning = false;
    bool isXMinusBtnDowning = false;
    bool isYMinusBtnDowning = false;
    bool isZMinusBtnDowning = false;

    bool isXRotPlusBtnDowning = false;
    bool isYRotPlusBtnDowning = false;
    bool isZRotPlusBtnDowning = false;
    bool isXRotMinusBtnDowning = false;
    bool isYRotMinusBtnDowning = false;
    bool isZRotMinusBtnDowning = false;
    public Transform endEffector;
    int currentStep = 0;
    public TMP_InputField durationInput;
    Vector3 originPos;
    Quaternion originRot;

    void Start()
    {
        x = endEffector.position.x;
        y = endEffector.position.y;
        z = endEffector.position.z;

        xRot = endEffector.eulerAngles.x;
        yRot = endEffector.eulerAngles.y;
        zRot = endEffector.eulerAngles.z;

        durationInput.text = "1";

        originPos = endEffector.localPosition;
        originRot = endEffector.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRobotOn || isStarted || isSequenceOn)
            return;

        if(!teachByToggle.isOn)
            UpdateEndEffector();
    }

    private void UpdateEndEffector()
    {
        UpdatePositionByButtons();

        UpdateRotationByButtons();

        xPosInput.text = x.ToString("0.00");
        yPosInput.text = y.ToString("0.00");
        zPosInput.text = z.ToString("0.00");

        xRotInput.text = xRot.ToString("0.00");
        yRotInput.text = yRot.ToString("0.00");
        zRotInput.text = zRot.ToString("0.00");

        endEffector.position = new Vector3(x, y, z);
        endEffector.rotation = Quaternion.Euler(xRot, yRot, zRot);
    }

    private void UpdatePositionByButtons()
    {
        if (isXPlusBtnDowning)
        {
            x += multiplier;
        }

        if (isYPlusBtnDowning)
        {
            y += multiplier;
        }

        if (isZPlusBtnDowning)
        {
            z += multiplier;
        }

        if (isXMinusBtnDowning)
        {
            x -= multiplier;
        }

        if (isYMinusBtnDowning)
        {
            y -= multiplier;
        }

        if (isZMinusBtnDowning)
        {
            z -= multiplier;
        }
    }

    private void UpdateRotationByButtons()
    {
        if (isXRotPlusBtnDowning)
        {
            xRot += rotMultiplier;
        }

        if (isYRotPlusBtnDowning)
        {
            yRot += rotMultiplier;
        }

        if (isZRotPlusBtnDowning)
        {
            zRot += rotMultiplier;
        }

        if (isXRotMinusBtnDowning)
        {
            xRot -= rotMultiplier;
        }

        if (isYRotMinusBtnDowning)
        {
            yRot -= rotMultiplier;
        }

        if (isZRotMinusBtnDowning)
        {
            zRot -= rotMultiplier;
        }
    }

    public void OnXPlusBtnDownEvent()
    {
        isXPlusBtnDowning = true;
    }

    public void OnXPlusBtnUpEvent()
    {
        isXPlusBtnDowning = false;
    }

    public void OnYPlusBtnDownEvent()
    {
        isYPlusBtnDowning = true;
    }

    public void OnYPlusBtnUpEvent()
    {
        isYPlusBtnDowning = false;
    }

    public void OnZPlusBtnDownEvent()
    {
        isZPlusBtnDowning = true;
    }

    public void OnZPlusBtnUpEvent()
    {
        isZPlusBtnDowning = false;
    }

    public void OnXMinusBtnDownEvent()
    {
        isXMinusBtnDowning = true;
    }

    public void OnXMinusBtnUpEvent()
    {
        isXMinusBtnDowning = false;
    }

    public void OnYMinusBtnDownEvent()
    {
        isYMinusBtnDowning = true;
    }

    public void OnYMinusBtnUpEvent()
    {
        isYMinusBtnDowning = false;
    }

    public void OnZMinusBtnDownEvent()
    {
        isZMinusBtnDowning = true;
    }

    public void OnZMinusBtnUpEvent()
    {
        isZMinusBtnDowning = false;
    }

    public void OnXRotPlusBtnDownEvent()
    {
        isXRotPlusBtnDowning = true;
    }

    public void OnXRotPlusBtnUpEvent()
    {
        isXRotPlusBtnDowning = false;
    }

    public void OnYRotPlusBtnDownEvent()
    {
        isYRotPlusBtnDowning = true;
    }

    public void OnYRotPlusBtnUpEvent()
    {
        isYRotPlusBtnDowning = false;
    }

    public void OnZRotPlusBtnDownEvent()
    {
        isZRotPlusBtnDowning = true;
    }

    public void OnZRotPlusBtnUpEvent()
    {
        isZRotPlusBtnDowning = false;
    }

    public void OnXRotMinusBtnDownEvent()
    {
        isXRotMinusBtnDowning = true;
    }

    public void OnXRotMinusBtnUpEvent()
    {
        isXRotMinusBtnDowning = false;
    }

    public void OnYRotMinusBtnDownEvent()
    {
        isYRotMinusBtnDowning = true;
    }

    public void OnYRotMinusBtnUpEvent()
    {
        isYRotMinusBtnDowning = false;
    }

    public void OnZRotMinusBtnDownEvent()
    {
        isZRotMinusBtnDowning = true;
    }

    public void OnZRotMinusBtnUpEvent()
    {
        isZRotMinusBtnDowning = false;
    }

    public void OnTeachBtnClkEvent()
    {
        string durationStr = durationInput.text;
        float _duration = 0;
        bool isParsed = float.TryParse(durationStr, out _duration);

        if (!isParsed) 
        {
            Debug.LogWarning("Duration 입력이 잘못되었습니다. float 형태로 넣어주세요.");
            return;
        }

        Step step = new Step()
        {
            stepNum = currentStep++,
            position = endEffector.localPosition,
            rotation = endEffector.localRotation,
            duration = _duration,
            isSuctionOn = suctionToggle.isOn
        };

        steps.Add(step);

        Debug.Log($"{step.stepNum}번째 Step이 저장되었습니다.");
    }

    public void OnStartBtnClkEvent()
    {
        isStarted = true;

        StartCoroutine(CoTotalSequenceMove());
    }

    public void OnCycleBtnClkEvent()
    {
        isSequenceOn = true;

        StartCoroutine(CoCycle());
    }

    public void OnStopBtnClkEvent()
    {
        isSequenceOn = false;
    }

    public void OnEmergencyBtnClkEvent()
    {
        isEmergency = !isEmergency;
        Debug.LogWarning("긴급정지버튼 클릭: " + isEmergency);
    }

    public void OnClearBtnClkEvent()
    {
        steps.Clear();
        Debug.Log("steps가 초기화 되었습니다.");
    }

    int sequenceCnt = 0;
    public void OnSaveAsSequenceBtnClkEvent()
    {
        if (steps.Count > 0)
        {
            List<Step> newSteps = new List<Step>(steps);

            if(sequenceCnt > 0)
            {
                List<Step> prevSeq = robotSequence.sequenceList[sequenceCnt - 1];
                Step lastStep = prevSeq[prevSeq.Count - 1];
                newSteps.Insert(0, lastStep);

                robotSequence.AddSequence(newSteps);
            }
            else
            {
                robotSequence.AddSequence(newSteps);
            }

            sequenceCnt++;

            steps.Clear();
            Debug.Log("시퀀스(steps)가 저장되었습니다. 기존 steps는 초기화 되었습니다.");
        }
        else
        {
            Debug.LogWarning("저장된 Step이 없습니다.");
        }
    }

    IEnumerator CoSequenceMove()
    {
        if (!isRobotOn)
        {
            Debug.LogWarning("로봇이 꺼져있습니다.");
            yield break;
        }

        if(steps.Count == 0)
        {
            Debug.LogWarning("저장된 Step이 없습니다.");
            yield break;
        }

        if(isEmergency)
        {
            Debug.LogWarning("E-Stop 버튼이 눌렸습니다. 초기화 해주세요.");
        }    

        Vector3    currentPos = endEffector.localPosition;
        Quaternion currentRot = endEffector.localRotation;

        Step currentStep = new Step() { position = currentPos, rotation = currentRot, duration = 1 };
        Step originStep = new Step() { position = originPos, rotation = originRot, duration = 1 };

        yield return CoMove(currentStep, originStep); // 원점으로 이동

        steps.Insert(0, originStep); // 원점이동 step을 step list의 0번째 Index에 추가

        for (int i = 0; i < steps.Count; i++)
        {
            if ((i + 1) == steps.Count)
                break;

            yield return CoMove(steps[i], steps[i + 1]);
        }

        steps.RemoveAt(0); // 원점이동 step을 step list에서 제거

        isStarted = false;

        x = endEffector.position.x; 
        y = endEffector.position.y; 
        z = endEffector.position.z; 
        xRot = endEffector.eulerAngles.x;
        yRot = endEffector.eulerAngles.y; 
        zRot = endEffector.eulerAngles.z; 
    }

    IEnumerator CoTotalSequenceMove()
    {
        if (!isRobotOn)
        {
            Debug.LogWarning("로봇이 꺼져있습니다.");
            yield break;
        }

        if (robotSequence.sequenceList.Count == 0)
        {
            Debug.LogWarning("저장된 Sequence가 없습니다.");
            yield break;
        }

        if (isEmergency)
        {
            Debug.LogWarning("E-Stop 버튼이 눌렸습니다. 초기화 해주세요.");
        }

        Vector3 currentPos = endEffector.localPosition;
        Quaternion currentRot = endEffector.localRotation;

        Step currentStep = new Step() { position = currentPos, rotation = currentRot, duration = 1 };
        Step originStep = new Step() { position = originPos, rotation = originRot, duration = 1 };

        yield return CoMove(currentStep, originStep); // 원점으로 이동

        robotSequence.sequenceList[0].Insert(0, originStep); // 원점이동 step을 step list의 0번째 Index에 추가

        for(int i = 0; i < robotSequence.sequenceList.Count; i++)
        {
            for (int j = 0; j < robotSequence.sequenceList[i].Count; j++)
            {
                if ((j + 1) == robotSequence.sequenceList[i].Count)
                    break;

                yield return CoMove(robotSequence.sequenceList[i][j], robotSequence.sequenceList[i][j + 1]);
            }
        }

        robotSequence.sequenceList[0].RemoveAt(0); // 원점이동 step을 step list에서 제거

        isStarted = false;

        x = endEffector.position.x;
        y = endEffector.position.y;
        z = endEffector.position.z;
        xRot = endEffector.eulerAngles.x;
        yRot = endEffector.eulerAngles.y;
        zRot = endEffector.eulerAngles.z;
    }

    IEnumerator CoCycle()
    {
        if (!isRobotOn)
        {
            Debug.LogWarning("로봇이 꺼져있습니다.");
            yield break;
        }

        if (steps.Count == 0)
        {
            Debug.LogWarning("저장된 Step이 없습니다.");
            yield break;
        }

        if (isEmergency)
        {
            Debug.LogWarning("E-Stop 버튼이 눌렸습니다. 초기화 해주세요.");
        }

        Vector3 currentPos = endEffector.localPosition;
        Quaternion currentRot = endEffector.localRotation;

        Step currentStep = new Step() { position = currentPos, rotation = currentRot, duration = 1 };
        Step originStep = new Step() { position = originPos, rotation = originRot, duration = 1 };

        yield return CoMove(currentStep, originStep); // 원점으로 이동

        steps.Insert(0, originStep); // 원점이동 step을 step list의 0번째 Index에 추가

        int repeatCount = 0;
        while (isSequenceOn)
        {
            if (isEmergency)
                break;

            for (int i = 0; i < steps.Count; i++)
            {
                if ((i + 1) == steps.Count)
                    break;

                yield return CoMove(steps[i], steps[i + 1]);
            }

            if (repeatCount == 0)
            {
                steps.Remove(originStep); // 원점이동 step을 step list에서 제거
                steps.Insert(0, steps[steps.Count - 1]);
            }

            repeatCount++;
        }

        steps.RemoveAt(0);
        repeatCount = 0;

        x = endEffector.position.x;
        y = endEffector.position.y;
        z = endEffector.position.z;
        xRot = endEffector.eulerAngles.x;
        yRot = endEffector.eulerAngles.y;
        zRot = endEffector.eulerAngles.z;
    }

    IEnumerator CoMove(Step prevStep, Step nextStep)
    {
        float currentTime = 0;

        while (!isEmergency)
        {
            currentTime += Time.deltaTime;

            if (currentTime > nextStep.duration)
                break;

            endEffector.localPosition = Vector3.Lerp(prevStep.position, nextStep.position, currentTime / nextStep.duration);
            endEffector.localRotation = Quaternion.Slerp(prevStep.rotation, nextStep.rotation, currentTime / nextStep.duration);

            isSuctionOn = nextStep.isSuctionOn;

            yield return new WaitForEndOfFrame();
        }
    }
}
