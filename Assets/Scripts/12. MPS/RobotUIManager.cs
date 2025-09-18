using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

// 목표: UI에 연결된 Position, Rotation 값을 바꿔서 로봇에 적용한다.
// 속성: OriginEndPos, OriginEndRot, 로봇전원상태, 석션상태토글
public class RobotUIManager : MonoBehaviour
{
    public bool isRobotOn = false;

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

    void Start()
    {
        x = endEffector.position.x;
        y = endEffector.position.y;
        z = endEffector.position.z;

        xRot = endEffector.eulerAngles.x;
        yRot = endEffector.eulerAngles.y;
        zRot = endEffector.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRobotOn)
            return;

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

}
