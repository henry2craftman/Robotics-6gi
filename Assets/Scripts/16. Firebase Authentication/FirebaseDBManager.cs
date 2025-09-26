using UnityEngine;
using Firebase;
using Firebase.Database;

namespace FirebaseAuthentication
{
    public class FirebaseDBManager : MonoBehaviour
    {
        public static FirebaseDBManager Instance;

        [SerializeField] string dbURL;
        DatabaseReference dbRef;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // 1. Firebase의 디폴트 객체에 url 전달
            FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);

            // 2. DB 래퍼런스 받아오기
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        }


        public async void AddUserInfo(string uID, string role)
        {
            await dbRef.Child("User").Child(uID).SetValueAsync(role).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"사용자 정보 추가 완료: UID[{uID}]를 역할[{role}](으)로 설정했습니다.");
                }
                else if (task.IsCanceled)
                {
                    Debug.LogWarning(task.Exception);
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception);
                }
            });
        }
    }
}
