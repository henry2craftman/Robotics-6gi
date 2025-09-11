using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float rotSpeed = 100;
    float yRot = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // RotateByEuler();

        RotateByQuaternion();
    }

    // eulerAngles의 짐벌락 현상으로 인해 카메라가 돌아감
    private void RotateByEuler()
    {
        float mouseY = Input.GetAxis("Mouse Y");

        float dx = mouseY * rotSpeed * Time.deltaTime;

        // 0 시계방향 회전 360
        // 0 반시계방향 1, 2, 3
        float currentX = transform.eulerAngles.x;
        if (currentX > 180)
            currentX -= 360;

        float clampedX = Mathf.Clamp(currentX - dx, -90, 90);

        transform.eulerAngles = new Vector3(currentX - dx, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void RotateByQuaternion()
    {
        float mouseY = Input.GetAxis("Mouse Y");

        yRot += mouseY * rotSpeed * Time.deltaTime;

        yRot = Mathf.Clamp(yRot, -90, 90);

        transform.rotation = Quaternion.Euler(-yRot, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
