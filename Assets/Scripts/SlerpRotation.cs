using UnityEngine;

public class SlerpRotation : MonoBehaviour
{
    public float time;          // 얼마동안 회전할지
    public float elapsedTime;   // 경과시간
    public float startAngle;
    public float endAngle;
    Quaternion startQ;          // 회전 값 저장
    Quaternion endQ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startQ = Quaternion.AngleAxis(startAngle, transform.forward);
        endQ = Quaternion.AngleAxis(endAngle, transform.forward);

        transform.rotation = endQ;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime; // 매 프레임의 시간을 더해줌

        if (elapsedTime > time)
        {
            elapsedTime = 0;
        }

        // 비율로 계산된 회전의 값을 선형으로 변환
        Quaternion q = Quaternion.Slerp(startQ, endQ, elapsedTime / time);

        transform.rotation = q; // q를 회전에 적용
    }
}
