using UnityEngine;

//로봇의 현재step의 정보 중 isSuctionOn이 true 라면 물체를 위치시킨다.
public class Suction : MonoBehaviour
{
    public RobotUIManager robotUIManager;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!(robotUIManager.CurrentSuctionOn))
        {
            if (rb != null)
            {
                rb.tag = "Plastic";
                rb.transform.SetParent(null);
                rb.isKinematic = false;
                rb = null;
            }
                
        }
        Debug.LogWarning("석션정보"+ robotUIManager.CurrentSuctionOn);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (robotUIManager == null)
        {
            Debug.LogWarning("RobotUIManager 참조가 없습니다.");
            return;
        }

        if (robotUIManager.CurrentSuctionOn)  // RobotUIManager에서 방송하는 현재 suction 상태 사용
        {
            if (other.CompareTag("Plastic"))
            {
                other.tag = "Palletizing";
                other.transform.SetParent(transform);
                other.transform.position = transform.position;
                other.transform.rotation = transform.rotation;

                rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
        }
    }
}
