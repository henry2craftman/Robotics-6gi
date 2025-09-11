using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float windSpeed;
    public Vector3 windDirection = new Vector3(1, 0, 0); // �ٶ��� ���� (��: X�� ����)
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
    //    if (rb.angularVelocity.magnitude > 0.1f) // ���� ��� ���� �����̰� ���� ��
    //    {
    //        // �ٶ��� ����� ������ ���Ͽ� ���� ���
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
