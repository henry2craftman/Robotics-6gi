using TMPro;
using UnityEngine;

// 목표: 3D공간에 있는 모든 UI를 제어한다.
public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    int score;

    // 로그인 관련 게임오브젝트
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public string id;
    public string pw;
    public GameObject controlPanel;
    public GameObject loginPanel;

    public void OnLoginBtnClkEvent()
    {
        if (idInput.text == id && pwInput.text == pw)
        {
            print("로그인이 완료되었습니다.");

            loginPanel.SetActive(false);
            controlPanel.SetActive(true);
        }
        else
        {
            print("id 또는 pw를 확인해 주세요.");
        }
    }

    public void OnBtnClkEvent()
    {
        print("버튼이 클릭되었습니다.");
        
        score++;

        scoreText.text = score.ToString();
    }
}
