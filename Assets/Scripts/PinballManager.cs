using UnityEngine;

public class PinballManager : MonoBehaviour
{
    public bool isGameStarted = false;
    public Transform ball;
    public float ballPower;
    public int totalPonint;
    private Vector3 originBallPos;
    private Quaternion originBallRot;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
        originBallPos = ball.transform.position;
        originBallRot = ball.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isGameStarted)
        {
            rb.AddForce(ball.transform.up * ballPower, ForceMode.Impulse);

            isGameStarted = true;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            print("Space를 눌러 공을 발사하세요!");
            ball.transform.position = originBallPos;
            ball.transform.rotation = originBallRot;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;

            isGameStarted = false;
        }
    }
}
