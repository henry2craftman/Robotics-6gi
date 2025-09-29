using Firebase;
using Firebase.Database;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


namespace MPSwithFirebase
{
    // 목표 : 공장설비들의 데이터를 Firebase Realtime Database에 동기화한다.
    //        (Master는 DB에 쓰기를 하고, Slave는 DB 정보를 읽기만 한다.
    // 속성 : DB의 Root reference(최상위 폴더), dbURL

    public class FirebaseDBManager : MonoBehaviour
    {
        public static FirebaseDBManager Instance;

        [SerializeField] string dbURL;
        DatabaseReference dbRef;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // 1. Firebase의 default 객체에 url 전달
            FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(dbURL);

            // 2. DB reference 받아오기
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        }

        // 1. Y 디바이스 정보들(int[] )
        // 2. X 디바이스 정보들(int[] )
        // 3. T, C, D 디바이스 정보들(int[] )
        // -> List<int[]> totalData

        public class TotalData
        {
            public int[] xDeviceBlocks;
            public int[] yDeviceBlocks;
            public int[] tDeviceBlocks;
            public int[] cDeviceBlocks;
            public int[] dDeviceBlocks;
        }

        IEnumerator RunFirebase()
        {
            yield return new WaitUntil(() => MxComponent.Instance.isConnected == true);

            Task.Run(UpdatePLCDataAsync);
        }

        // Firebase DB의 *CRUD에 Update 해주는 메서드
        // *DB 접근을 위한 기본기능 Create Read Update Delete
        public async void UpdatePLCDataAsync()
        {
            TotalData totalData = new TotalData();

            while (MxComponent.Instance.isConnected)
            {
                //상호배제(Mutex)로 공유자원에 여러 스레드가 접근하는 것을 순서화 해준다.
                lock (MxComponent.Instance.lockObj)
                {
                    totalData.xDeviceBlocks = MxComponent.Instance.xDeviceBlocks;
                    totalData.yDeviceBlocks = MxComponent.Instance.yDeviceBlocks;
                }

                // Object -> Json
                string json = JsonUtility.ToJson(totalData);

                await dbRef.SetRawJsonValueAsync(json);

                await Task.Delay(10);
            }
        }

        public async void ReadPLCDataAsync()
        {
            while(MxComponent.Instance.isConnected)
            {
                await dbRef.GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        if (snapshot.Exists)
                        {
                            string json = snapshot.GetRawJsonValue();

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