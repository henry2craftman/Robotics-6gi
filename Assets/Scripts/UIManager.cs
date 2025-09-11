using TMPro;
using UnityEngine;

// ��ǥ: 3D������ �ִ� ��� UI�� �����Ѵ�.
public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    int score;

    // �α��� ���� ���ӿ�����Ʈ
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
            print("�α����� �Ϸ�Ǿ����ϴ�.");

            loginPanel.SetActive(false);
            controlPanel.SetActive(true);
        }
        else
        {
            print("id �Ǵ� pw�� Ȯ���� �ּ���.");
        }
    }

    public void OnBtnClkEvent()
    {
        print("��ư�� Ŭ���Ǿ����ϴ�.");
        
        score++;

        scoreText.text = score.ToString();
    }
}
