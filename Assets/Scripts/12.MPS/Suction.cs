using UnityEngine;

// 목표: Robot의 현재 Step의 정보 중 isSuctionOn이 true라면 물체를 위치시킨다.
public class Suction : MonoBehaviour
{
    public RobotUI.RobotUIManager robotUIManager;
    Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        if (!robotUIManager.isSuctionOn)
        {
            if(rb != null)
            {
                rb.tag = "Plastic";
                rb.transform.SetParent(null);
                rb.isKinematic = false;
                rb = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(robotUIManager.isSuctionOn)
        {
            print(other.name);
            // 물체를 나 자신에게 붙인다.
            if(other.tag == "Plastic")
            {
                other.tag = "Palletizing";

                other.transform.SetParent(transform);
                other.transform.position = transform.position;
                other.transform.up = transform.forward;

                rb = other.GetComponent<Rigidbody>();
                rb.isKinematic = true;
            }
        }
    }
}
