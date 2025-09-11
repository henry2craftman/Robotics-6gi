using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float windSpeed;
    public Vector3 windDirection = new Vector3(1, 0, 0); // 바람의 방향 (예: X축 방향)
    Rigidbody rb;
    AudioSource audio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }
    //void FixedUpdate()
    //{
    //    if (rb.angularVelocity.magnitude > 0.1f) // 공이 어느 정도 움직이고 있을 때
    //    {
    //        // 바람의 방향과 강도를 곱하여 힘을 계산
    //        Vector3 windForce = windDirection.normalized * windSpeed;

    //        rb.AddForce(windForce);
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        audio.Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, windDirection.normalized * windSpeed);
    }
}
