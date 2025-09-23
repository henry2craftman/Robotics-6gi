using UnityEngine;
using Firebase.Database;
using System.Text;
using System;
using System.Threading.Tasks;
using TMPro;
using System.Collections.Generic;

// 목표: DB의 기본기능 CRUD를 만든다.
// 속성: UserData 클래스, CreateUser 기능, ReadUser 기능, UpdateUserScore 기능, DeleteUser 기능
public class FirebaseDBManager : MonoBehaviour
{
    public class UserData
    {
        public string name;
        public int score;
    }

    [SerializeField] string dbURL;
    DatabaseReference reference;

    public TMP_InputField userNameInput;
    public TMP_InputField userScoreInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        // Database의 RootRefernece 참조하기
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        /*reference.Child("userData").Child("items").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                string json = snapshot.GetRawJsonValue();

                byte[] utf8Bytes = Encoding.UTF8.GetBytes(json);

                json = Encoding.UTF8.GetString(utf8Bytes);

                print(json);
            }
        });*/

        //await CreateUserAsync("Sangho", 100);

        //await ReadUser("Sangho");

        //await UpdateUserScoreAsync("Sangho", 500);

        //await DeleteUserAsync("TY");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 1. Create
    public async Task CreateUserAsync(string _name, int _score)
    {
        UserData newUser = new UserData() { name = _name, score = _score };

        // Serialization: 객체 -> json
        string json = JsonUtility.ToJson(newUser);

        // userData에 내용을 저장
        await reference.Child("userData").Child(_name).SetRawJsonValueAsync(json);

        Debug.Log("[Create] 유저 생성 완료. " + _name);
    }

    // 2. Read
    public async Task<UserData> ReadUser(string _name)
    {
        DataSnapshot snapShot = await reference.Child("userData").Child(_name).GetValueAsync();

        if (snapShot.Exists)
        {
            string json = snapShot.GetRawJsonValue();

            // DeSerialization: Json -> 객체
            UserData user = JsonUtility.FromJson<UserData>(json);

            Debug.Log($"[Read] {user.name}는 {user.score}점 입니다.");

            return user;
        }
        else
        {
            Debug.LogWarning("[Read] 유저를 찾을 수 없음. " + _name);

            return null;
        }
    }

    // 3. Update
    public async Task UpdateUserScoreAsync(string _name, int _score)
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();

        updates["score"] = _score;

        await reference.Child("userData").Child(_name).UpdateChildrenAsync(updates);

        Debug.Log($"[Update] 업데이트가 완료되었습니다. {_name}의 새 점수는 {_score}");
    }

    // 4. Delete
    public async Task DeleteUserAsync(string _name)
    {
        UserData user = await ReadUser(_name);

        if (user != null)
        {
            await reference.Child("userData").Child(_name).RemoveValueAsync();

            Debug.Log("[Delete] 유저 삭제 완료. " + _name);
        }
    }

    public async void OnCreateUserBt12nClkEvent()
    {
        if(string.IsNullOrEmpty(userNameInput.text) || string.IsNullOrEmpty(userScoreInput.text))
        {
            Debug.LogWarning("유저이름과 스코어는 필수입니다.");
            return;
        }

        await CreateUserAsync(userNameInput.text, int.Parse(userScoreInput.text));

        Debug.Log(userNameInput.text + " 생성완료");
    }

    public async void OnReadUserBtnClkEvent()
    {
        if (string.IsNullOrEmpty(userNameInput.text))
        {
            Debug.LogWarning("유저이름을 입력해 주세요.");
            return;
        }

        UserData user = await ReadUser(userNameInput.text);

        if (user != null)
        {
            Debug.Log($"{user.name}은 {user.score}점 입니다.");

            userScoreInput.text = user.score.ToString();
        }
        else
        {
            Debug.Log("유저 정보를 찾을 수 없습니다.");
        }
    }

    public async void OnUpdateBtnClkEvent()
    {
        if (string.IsNullOrEmpty(userNameInput.text) || string.IsNullOrEmpty(userScoreInput.text))
        {
            Debug.LogWarning("유저이름과 스코어는 필수입니다.");
            return;
        }

        await UpdateUserScoreAsync(userNameInput.text, int.Parse(userScoreInput.text));
    }

    public async void OnDeleteBtnClkEvent()
    {
        if (string.IsNullOrEmpty(userNameInput.text))
        {
            Debug.LogWarning("유저이름을 입력해 주세요.");
            return;
        }

        await DeleteUserAsync(userNameInput.text);
    }
}
