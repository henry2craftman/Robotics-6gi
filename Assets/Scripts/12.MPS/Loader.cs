using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

namespace MPS
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] GameObject[] objPrefabs;


        public bool isLoadSignal = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void OnLoadBtnClkEvent()
        {
            int rand = Random.Range(0, objPrefabs.Length);
            GameObject obj = Instantiate(objPrefabs[rand]);
            obj.transform.position = transform.position;
            obj.AddComponent<Rigidbody>();
        }

    }
}
