using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

// ��ǥ: async/await ����� ����Ͽ� �񵿱� �۾� ���� ����� (Coroutine �Լ��� �뵵 ��)
public class AsyncAwaitManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(CoCountDown());
        }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            GetScoreAsync();
        }
    }

    IEnumerator CoCountDown()
    {
        Debug.Log("3..");

        yield return new WaitForSeconds(1);

        Debug.Log("2..");

        yield return new WaitForSeconds(1);

        Debug.Log("1..");

        yield return new WaitForSeconds(1);

        Debug.Log("Go");
    }

    async void GetScoreAsync()
    {
        int score = await FetchScoreFromServerAsync();

        Debug.Log($"�������� �޾ƿ� ������ {score}�Դϴ�.");
    }

    // ������ ��û�� ������ �Լ�

    async Task<int> FetchScoreFromServerAsync()
    {
        await Task.Delay(2000);

        return 100;
    }
}
