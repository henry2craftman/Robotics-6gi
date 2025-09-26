using Firebase;
using Firebase.Database;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;

namespace MPSwithFirebase
{
    // 목표: 공장설비들의 데이터를 Firebase Realtime Database에 동기화한다.
    //       (Master는 DB에 쓰기를 하고, Slave는 DB의 정보를 읽기만 한다.)
    // 속성: DB의 Root reference(최상위폴더), dbURL
    public class FirebaseDBManager : MonoBehaviour
    {
        public static FirebaseDBManager Instance;

        [SerializeField] string dbURL;
        DatabaseReference dbRef;

        private void Awake()
        {
            if (Instance == null)
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

        // 1. Y 디바이스 정보들(int[])
        // 2. X 디바이스 정보들(int[])
        // 3. T, C, D 디바이스 정보들(int[])
        // -> List<int[]> totalData
        public class TotalData
        {
            public int[] xDeviceBlocks;
            public int[] yDeviceBlocks;
            public int[] tDeviceBlocks;
            public int[] cDeviceBlocks;
            public int[] dDeviceBlocks;
        }

        // Firebase DB에 CRUD의 Update해주는 메서드
        // DB 접근을 위한 기본기능 Create Read Update Delete
        int count = 0;
        public async void UpdatePLCDataAsync()
        {
            TotalData totalData = new TotalData();

            while (MxComponent.Instance.isConnected)
            {
                // 상호배제(Mutex) 로 공유자원에 여러 스레드가 접근하는 것을 순서화 해준다.
                lock (MxComponent.Instance.lockObj)
                {
                    totalData.xDeviceBlocks = MxComponent.Instance.xDeviceBlocks;
                    totalData.yDeviceBlocks = MxComponent.Instance.yDeviceBlocks;
                }
                // Object -> Json
                string json = JsonConvert.SerializeObject(totalData);

                await dbRef.SetRawJsonValueAsync(json);

                await Task.Delay(10);
            }
        }

        public async void ReadPLCDataAsync()
        {
            //await dbRef.GetValueAsync().ContinueWith(Test);

            //void Test(Task<DataSnapshot> task)
            //{

            //}
            while(MxComponent.Instance.isConnected)
            {
                await dbRef.GetValueAsync().ContinueWith(async task =>
                {
                    if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        if (snapshot.Exists)
                        {
                            string json = snapshot.GetRawJsonValue();

                            // Json -> Object
                            TotalData totalData = JsonUtility.FromJson<TotalData>(json);

                            MxComponent.Instance.yDeviceBlocks = totalData.yDeviceBlocks;
                        }
                    }
                    else if (task.IsCanceled)
                    {
                        Debug.LogWarning(task.Exception);
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogWarning(task.Exception);
                    }

                });
                
                await Task.Delay(10);
            }
        }
    }
}