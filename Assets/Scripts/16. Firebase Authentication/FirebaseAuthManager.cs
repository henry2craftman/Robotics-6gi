using Firebase.Auth;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// 목표: Firebase Authentication SDK를 사용하여 이메일 회원가입을 하고, 로그인한다.
// 속성: Firebase Auth 객체
// 회원가입 패널: ID(email)입력 인풋필드, PW입력 인풋필드, PW확인 인풋필드, 회원가입버튼, 나가기버튼
// 로그인 패널: ID(email)입력 인풋필드, PW입력 인풋필드, 로그인버튼, 나가기버튼
// 인증확인 팝업 패널
public class FirebaseAuthManager : MonoBehaviour
{
    [Header("회원가입 패널")]
    public GameObject signUpPanel;
    public TMP_InputField signUpEmailInput;
    public TMP_InputField signUpPWInput;
    public TMP_InputField signUpPWCheckInput;

    [Header("로그인 패널")]
    public GameObject loginPanel;
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPWInput;

    [Header("이메일 인증확인 팝업 패널")]
    public GameObject popupPanel;
    public TMP_Text popupText;

    FirebaseAuth auth;
    FirebaseUser user;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnAuthStateChanged;
    }

    // 로그인 상태 변경 감지
    void OnAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = (user != auth.CurrentUser) && (auth.CurrentUser != null);

            if (!signedIn && user != null)
            {
                Debug.Log($"로그아웃: {user.UserId}");
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"로그인: {user.UserId}");
            }
        }
    }

    // 1. 회원가입 기능
    public async void OnSignUpBtnClkEvent()
    {
        string email = signUpEmailInput.text;
        string password = signUpPWInput.text;
        string passwordCheck = signUpPWCheckInput.text;

        // 입력이 비어있는지 확인
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))
        {
            Debug.LogWarning("이메일과 비밀번호를 입력해 주세요.");
            return;
        }

        // 비밀번호 유효성검사
        if(password != passwordCheck)
        {
            Debug.LogWarning("비밀번호가 일치하지 않습니다.");
            return;
        }

        // 회원가입
        await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            // 회원가입 완료
            if(task.IsCompleted)
            {
                user = task.Result.User;

                Debug.Log($"회원가입 성공: {user.Email}");
            }
            else if(task.IsCanceled)
            {
                Debug.LogWarning(task.Exception);

                return;
            }
            else if(task.IsFaulted)
            {
                Debug.LogWarning(task.Exception);

                return;
            }
        });

        SendVerificationEmailAsync(user);

        loginPanel.SetActive(true);
        signUpPanel.SetActive(false);
    }

    // 회원가입 취소 기능
    public void OnSignUpCancelBtnClkEvent()
    {
        loginPanel.SetActive(true);
        signUpPanel.SetActive(false);
    }

    // 2. 로그인 기능
    public async void OnLoginBtnClkEvent()
    {
        string email = loginEmailInput.text;
        string password = loginPWInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("이메일과 비밀번호를 입력해 주세요.");

            return;
        }

        await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                user = task.Result.User;
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning(task.Exception);

                return;
            }
            else if (task.IsFaulted)
            {
                Debug.LogWarning(task.Exception);

                return;
            }
        });

        // 이메일 인증확인
        if (!user.IsEmailVerified)
        {
            StartCoroutine(CoTurnOnPopupPanel($"Check your email: {user.Email}"));

            auth.SignOut();

            return;
        }

        Debug.Log($"로그인 성공: {user.Email}");

        loginPanel.SetActive(false);

        SceneManager.LoadScene("12-2. MPS with Firebase"); // Loading Scene이 따로 있을 때 LoadSceneAsync
    }

    // 회원가입 패널로 가는 기능
    public void OnMoveToSignUpBtnClkEvent()
    {
        loginPanel.SetActive(false);
        signUpPanel.SetActive(true);
    }

    // 프로그램 종료 기능
    public void OnExitBtnClkEvent()
    {
        Debug.Log("프로그램이 종료됩니다...");

        Application.Quit();
    }

    /// <summary>
    /// 인증 이메일을 보내는 기능
    /// </summary>
    /// <param name="targetUser">인증 이메일을 해당 유저에 보냅니다.</param>
    public async void SendVerificationEmailAsync(FirebaseUser targetUser)
    {
        await targetUser.SendEmailVerificationAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"인증 이메일 전송");
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning(task.Exception);

                return;
            }
            else if (task.IsFaulted)
            {
                Debug.LogWarning(task.Exception);

                return;
            }
        });

        StartCoroutine(CoTurnOnPopupPanel($"{targetUser.Email}에서 인증 확인을 눌러주세요."));
    }

    IEnumerator CoTurnOnPopupPanel(string msg)
    {
        popupPanel.SetActive(true);
        popupText.text = msg;

        yield return new WaitForSeconds(3);

        popupPanel.SetActive(false);
    }
}
