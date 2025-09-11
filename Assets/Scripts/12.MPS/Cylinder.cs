using System.Collections;
using UnityEngine;

// ��ǥ : �Ǹ��� Rod�� minRange���� maxRange�� �̵���Ų��.
// �Ӽ� : �Ǹ��� Rod�� transform, minRange, maxRange, speed
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Transform rod;            // private �Ӽ��ε� inspector â�� ����.
    [SerializeField] float minRange;

    public float MinRange { get => minRange; set => value = minRange; } // Unity���� property�� inspector â�� ����������.
    [SerializeField] float maxRange;
    [SerializeField] float speed;

    // CylinderForward ��ư�� ������ Cylinder ����.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28215f);
        Vector3 endPos = new Vector3(minRange, 4.25f, 6.28215f);
        StartCoroutine(CoMoveCylinder(startPos, endPos));
    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to)
    {
        while (true)
        {
            Vector3 dir = to - rod.localPosition;

            float distance = dir.magnitude;

            if(distance < 0.1f)
            {
                break;
            }

            dir.Normalize();

            rod.localPosition += dir * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        
    }
}
