using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RobotUIManager : MonoBehaviour
{
    [Serializable]
    public struct Step
    {
        public int stepNum;
        public Vector3 position;
        public Quaternion rotation;
        public float duration;
        public bool isSuctionOn;

        public bool isSelected; // Inspector 체크박스 용
    }

    [Serializable]
    public class RobotSequence
    {
        public List<Step> sequenceList = new List<Step>();
    }

    [Header("로봇 시퀀스 리스트")]
    public List<RobotSequence> robotSequenceList = new List<RobotSequence>();

    [Header("현재 실행 준비된 Steps (Inspector에서 개별 Step 선택 가능)")]
    public List<Step> steps = new List<Step>();

    public bool isRobotOn = false;
    public bool isEmergency = false;

    private Coroutine startCoroutine = null;
    private Coroutine cycleCoroutine = null;
    private bool isRunningStart = false;
    private bool isRunningCycle = false;

    private int currentSequenceIndex = 0; // robotSequenceList의 현재 실행 중 시퀀스 인덱스

    public Vector3 originPos;
    public Quaternion originRot;

    public TMP_InputField xPosInput;
    public TMP_InputField yPosInput;
    public TMP_InputField zPosInput;
    public TMP_InputField xRotInput;
    public TMP_InputField yRotInput;
    public TMP_InputField zRotInput;
    public Toggle suctionToggle;
    public TMP_InputField durationInput;

    public TMP_Text currentStepText; // 현재 Step UI 출력용 텍스트 (Inspector에서 할당)

    public float positionStep = 0.01f;
    public float rotationStep = 0.1f;

    bool isXPlusBtnDown = false;
    bool isYPlusBtnDown = false;
    bool isZPlusBtnDown = false;
    bool isXMinusBtnDown = false;
    bool isYMinusBtnDown = false;
    bool isZMinusBtnDown = false;
    bool isXRotPlusBtnDown = false;
    bool isYRotPlusBtnDown = false;
    bool isZRotPlusBtnDown = false;
    bool isXRotMinusBtnDown = false;
    bool isYRotMinusBtnDown = false;
    bool isZRotMinusBtnDown = false;

    public Transform endEffector;

    float x, y, z;
    float xRot, yRot, zRot;

    int stepCount = 0;

    public bool CurrentSuctionOn { get; private set; } = false;

    public event Action<bool> OnSuctionStateChanged;

    void Start()
    {
        originPos = endEffector.localPosition;
        originRot = endEffector.localRotation;

        Vector3 pos = endEffector.localPosition;
        Vector3 rot = endEffector.localEulerAngles;

        x = pos.x; y = pos.y; z = pos.z;
        xRot = rot.x; yRot = rot.y; zRot = rot.z;

        durationInput.text = "1";

        Debug.Log("[Start] 초기 currentSequenceIndex=0으로 설정");
        currentSequenceIndex = 0;

        UpdateCurrentStepText(-1);
    }

    void Update()
    {
        if (!isRobotOn || isEmergency || isRunningStart || isRunningCycle)
            return;

        UpdateManualControl();
    }

    void UpdateManualControl()
    {
        if (isXPlusBtnDown) x += positionStep;
        if (isYPlusBtnDown) y += positionStep;
        if (isZPlusBtnDown) z += positionStep;

        if (isXMinusBtnDown) x -= positionStep;
        if (isYMinusBtnDown) y -= positionStep;
        if (isZMinusBtnDown) z -= positionStep;

        if (isXRotPlusBtnDown) xRot += rotationStep;
        if (isYRotPlusBtnDown) yRot += rotationStep;
        if (isZRotPlusBtnDown) zRot += rotationStep;

        if (isXRotMinusBtnDown) xRot -= rotationStep;
        if (isYRotMinusBtnDown) yRot -= rotationStep;
        if (isZRotMinusBtnDown) zRot -= rotationStep;

        ApplyPositionRotation(x, y, z, xRot, yRot, zRot);
    }

    void ApplyPositionRotation(float px, float py, float pz, float rx, float ry, float rz)
    {
        Vector3 newPos = new Vector3(px, py, pz);
        Quaternion newRot = Quaternion.Euler(rx, ry, rz);

        endEffector.localPosition = newPos;
        endEffector.localRotation = newRot;

        x = px; y = py; z = pz;
        xRot = rx; yRot = ry; zRot = rz;

        UpdateInputFields();
    }

    void UpdateInputFields()
    {
        xPosInput.text = x.ToString("F2");
        yPosInput.text = y.ToString("F2");
        zPosInput.text = z.ToString("F2");

        xRotInput.text = xRot.ToString("F2");
        yRotInput.text = yRot.ToString("F2");
        zRotInput.text = zRot.ToString("F2");
    }

    void UpdateCurrentStepText(int stepIndex)
    {
        if (currentStepText == null)
            return;

        if (stepIndex >= 0 && stepIndex < steps.Count)
            currentStepText.text = $"현재 Step: {stepIndex} (ID: {steps[stepIndex].stepNum})";
        else
            currentStepText.text = "현재 Step: 없음";
    }

    #region 버튼 눌림 이벤트
    public void OnXPlusBtnDown() => isXPlusBtnDown = true;
    public void OnXPlusBtnUp() => isXPlusBtnDown = false;
    public void OnYPlusBtnDown() => isYPlusBtnDown = true;
    public void OnYPlusBtnUp() => isYPlusBtnDown = false;
    public void OnZPlusBtnDown() => isZPlusBtnDown = true;
    public void OnZPlusBtnUp() => isZPlusBtnDown = false;

    public void OnXMinusBtnDown() => isXMinusBtnDown = true;
    public void OnXMinusBtnUp() => isXMinusBtnDown = false;
    public void OnYMinusBtnDown() => isYMinusBtnDown = true;
    public void OnYMinusBtnUp() => isYMinusBtnDown = false;
    public void OnZMinusBtnDown() => isZMinusBtnDown = true;
    public void OnZMinusBtnUp() => isZMinusBtnDown = false;

    public void OnXRotPlusBtnDown() => isXRotPlusBtnDown = true;
    public void OnXRotPlusBtnUp() => isXRotPlusBtnDown = false;
    public void OnYRotPlusBtnDown() => isYRotPlusBtnDown = true;
    public void OnYRotPlusBtnUp() => isYRotPlusBtnDown = false;
    public void OnZRotPlusBtnDown() => isZRotPlusBtnDown = true;
    public void OnZRotPlusBtnUp() => isZRotPlusBtnDown = false;

    public void OnXRotMinusBtnDown() => isXRotMinusBtnDown = true;
    public void OnXRotMinusBtnUp() => isXRotMinusBtnDown = false;
    public void OnYRotMinusBtnDown() => isYRotMinusBtnDown = true;
    public void OnYRotMinusBtnUp() => isYRotMinusBtnDown = false;
    public void OnZRotMinusBtnDown() => isZRotMinusBtnDown = true;
    public void OnZRotMinusBtnUp() => isZRotMinusBtnDown = false;
    #endregion

    public void OnToggleSW(bool isOn)
    {
        CurrentSuctionOn = isOn;
        OnSuctionStateChanged?.Invoke(isOn);
        Debug.Log($"Toggle Switch 상태 변경: {(isOn ? "ON" : "OFF")}");
    }

    public void OnTeachBtnClick()
    {
        if (!float.TryParse(durationInput.text, out float duration) || duration <= 0)
        {
            Debug.LogWarning("Duration 입력 값이 올바르지 않습니다.");
            return;
        }
        steps.Add(new Step
        {
            stepNum = stepCount++,
            position = endEffector.localPosition,
            rotation = endEffector.localRotation,
            duration = duration,
            isSuctionOn = suctionToggle.isOn,
            isSelected = false
        });
        Debug.Log($"Step {stepCount - 1} 저장 완료.");
    }

    public void OnStartBtnClick()
    {
        if (!isRobotOn)
        {
            Debug.LogWarning("로봇 전원이 꺼져있습니다.");
            return;
        }
        if (robotSequenceList.Count == 0)
        {
            Debug.LogWarning("실행할 시퀀스가 없습니다.");
            return;
        }
        if (currentSequenceIndex >= robotSequenceList.Count)
        {
            Debug.Log("[OnStartBtnClick] 모든 시퀀스 실행 완료, 0번 시퀀스로 순환");
            currentSequenceIndex = 0;
        }

        StopAllMotion();

        // 현재 시퀀스 로드
        steps = new List<Step>(robotSequenceList[currentSequenceIndex].sequenceList);
        stepCount = steps.Count;
        Debug.Log($"[OnStartBtnClick] 시퀀스 {currentSequenceIndex} 불러옴. Step 개수: {steps.Count}");

        isEmergency = false;
        isRunningStart = true;

        startCoroutine = StartCoroutine(RunSequenceBySteps());

        // 다음 시퀀스 준비 (다음 호출 때 실행)
        currentSequenceIndex++;
    }

    IEnumerator RunSequenceBySteps()
    {
        Debug.Log("[RunSequenceBySteps] 시작");

        for (int i = 0; i < steps.Count; i++)
        {
            if (isEmergency || !isRunningStart)
                break;

            Step fromStep = i == 0 ? new Step { position = endEffector.localPosition, rotation = endEffector.localRotation, duration = 0 } : steps[i - 1];
            Step toStep = steps[i];

            if (CurrentSuctionOn != toStep.isSuctionOn)
            {
                CurrentSuctionOn = toStep.isSuctionOn;
                OnSuctionStateChanged?.Invoke(CurrentSuctionOn);
            }

            Debug.Log($"[RunSequenceBySteps] Step 실행: {i}, stepNum: {toStep.stepNum}");
            UpdateCurrentStepText(i);

            yield return MoveBetweenSteps(fromStep, toStep);
        }

        isRunningStart = false;
        UpdateCurrentStepText(-1);
        Debug.Log("[RunSequenceBySteps] 완료");
    }

    public void OnCycleBtnClick()
    {
        if (!isRobotOn)
        {
            Debug.LogWarning("로봇 전원이 꺼져있습니다.");
            return;
        }
        if (steps.Count == 0)
        {
            Debug.LogWarning("Step이 없습니다.");
            return;
        }
        StopAllMotion();

        isEmergency = false;
        isRunningCycle = true;

        cycleCoroutine = StartCoroutine(RunCycle());
    }

    public void OnStopBtnClick()
    {
        StopAllMotion();
        Debug.Log("동작 정지됨.");
        UpdateCurrentStepText(-1);
    }

    public void OnEmergencyBtnClick()
    {
        StopAllMotion();
        isEmergency = true;
        Debug.LogWarning("비상정지 활성화됨.");
        UpdateCurrentStepText(-1);
    }

    public void OnClearBtnClick()
    {
        steps.Clear();
        stepCount = 0;
        Debug.Log("모든 Step이 삭제되었습니다.");
        UpdateCurrentStepText(-1);
    }

    public void OnSaveCurrentStepsToSequence()
    {
        if (steps.Count == 0)
        {
            Debug.LogWarning("저장할 Step이 없습니다.");
            return;
        }
        if (currentSequenceIndex > 0 && currentSequenceIndex <= robotSequenceList.Count)
        {
            robotSequenceList[currentSequenceIndex - 1].sequenceList = new List<Step>(steps);
            Debug.Log($"시퀀스 {currentSequenceIndex - 1} 덮어씀 완료.");
        }
        else
        {
            RobotSequence newSeq = new RobotSequence();
            newSeq.sequenceList = new List<Step>(steps);
            robotSequenceList.Add(newSeq);
            currentSequenceIndex = robotSequenceList.Count;
            Debug.Log("새 시퀀스로 저장 완료.");
        }
    }

    void StopAllMotion()
    {
        Debug.Log("[StopAllMotion] 호출됨");
        if (startCoroutine != null)
        {
            StopCoroutine(startCoroutine);
            startCoroutine = null;
            isRunningStart = false;
            Debug.Log("[StopAllMotion] startCoroutine 중지");
        }
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
            cycleCoroutine = null;
            isRunningCycle = false;
            Debug.Log("[StopAllMotion] cycleCoroutine 중지");
        }
    }

    IEnumerator RunCycle()
    {
        while (isRunningCycle && !isEmergency)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                if (isEmergency || !isRunningCycle)
                    yield break;

                Step fromStep = i == 0 ? new Step { position = endEffector.localPosition, rotation = endEffector.localRotation, duration = 0 } : steps[i - 1];
                Step toStep = steps[i];

                if (CurrentSuctionOn != toStep.isSuctionOn)
                {
                    CurrentSuctionOn = toStep.isSuctionOn;
                    OnSuctionStateChanged?.Invoke(CurrentSuctionOn);
                }

                yield return MoveBetweenSteps(fromStep, toStep);
            }
        }
        Debug.Log("Cycle 종료.");
    }

    IEnumerator MoveBetweenSteps(Step fromStep, Step toStep)
    {
        float startTime = Time.time;
        float duration = Mathf.Max(toStep.duration, 0.01f);

        Vector3 startPos = fromStep.position;
        Quaternion startRot = fromStep.rotation;
        Vector3 endPos = toStep.position;
        Quaternion endRot = toStep.rotation;

        while (true)
        {
            float elapsed = Time.time - startTime;
            if (elapsed >= duration || isEmergency || (!isRunningStart && !isRunningCycle) || !isRobotOn)
                break;

            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            Quaternion currentRot = Quaternion.Slerp(startRot, endRot, t);

            ApplyPositionRotation(currentPos.x, currentPos.y, currentPos.z,
                currentRot.eulerAngles.x, currentRot.eulerAngles.y, currentRot.eulerAngles.z);

            yield return null;
        }

        if (isRobotOn && !isEmergency && (isRunningStart || isRunningCycle))
        {
            ApplyPositionRotation(endPos.x, endPos.y, endPos.z,
                endRot.eulerAngles.x, endRot.eulerAngles.y, endRot.eulerAngles.z);
        }
    }
}
