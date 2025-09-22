using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

namespace MPS
{
    using System.Collections;
    using UnityEngine;

    public class Loader : MonoBehaviour
    {
        [SerializeField] GameObject[] objPrefabs;
        public bool isLoadedSignal = false;

        public void OnLoadBtnClkEvent()
        {
            int rand = Random.Range(0, objPrefabs.Length);

            GameObject obj = Instantiate(objPrefabs[rand]);

            obj.transform.position = transform.position;
        }

        private void Start()
        {
            StartCoroutine(CoRoof());
        }

        IEnumerator CoRoof()
        {
            while (true)
            {
                if (objPrefabs == null || objPrefabs.Length == 0)
                {
                    Debug.LogError("objPrefabs 배열이 비었거나 null입니다.");
                    yield break;
                }

                int rand = Random.Range(0, objPrefabs.Length);
                Debug.Log("생성할 인덱스: " + rand);

                GameObject obj = Instantiate(objPrefabs[rand], transform.position, Quaternion.identity);
                Debug.Log("오브젝트 생성됨: " + obj.name);

                yield return new WaitForSeconds(3);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Metal" || other.tag == "Plastic")
            {
                isLoadedSignal = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Metal" || other.tag == "Plastic")
            {
                isLoadedSignal = false;
            }
        }
    }
}
