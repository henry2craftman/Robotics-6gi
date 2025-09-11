using UnityEngine;

public class Sensor : MonoBehaviour
{
    public Material[] materials;
    MeshRenderer mr;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name + " ��������!");

        //mr.materials[0].color = Color.green;
        mr.materials[0].color = new Color(0, 1, 0, 0.5f);
    }

    private void OnTriggerStay(Collider other)
    {
        print(other.gameObject.name + " ������...");
    }

    private void OnTriggerExit(Collider other)
    {
        print(other.gameObject.name + " ���� ����");

        //mr.materials[0].color = Color.red;
        mr.materials[0].color = new Color(1, 0, 0, 0.5f);
    }
}
