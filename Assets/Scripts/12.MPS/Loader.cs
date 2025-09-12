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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
