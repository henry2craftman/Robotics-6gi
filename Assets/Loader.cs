using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using UnityEditor;
using UnityEngine;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Metal" || other.tag == "Plastic")
        {
            isLoadSignal = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Metal" || other.tag == "Plastic")
        {
            isLoadSignal = false;
        }
    }

}