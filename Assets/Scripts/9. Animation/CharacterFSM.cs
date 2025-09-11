using UnityEngine;

// 목표: 캐릭터를 FSM 다이어그램(Animator의 형태)에 따라 움직이고 싶다.
// 속성: 현재 캐릭터의 상태, 속도
public class CharacterFSM : MonoBehaviour
{
    public enum State
    {
        Idle,
        Move,
        Point
    }

    public State state = State.Idle; // 현태 상태
    public float speed = 3;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Point"))
        {
            return;
        }

        Move();

        Point();

        AnimationFSM();
    }

    private void AnimationFSM()
    {
        switch (state)
        {
            case State.Idle:
                animator.SetTrigger("Idle");
                break;
            case State.Move:
                animator.SetTrigger("Walk");
                break;
            case State.Point:
                animator.SetTrigger("Point");
                break;
        }
    }

    private void Point()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            state = State.Point;
        }
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            Vector3 dir = (transform.right * h) + (transform.forward * v);

            transform.position += dir * speed * Time.deltaTime;

            state = State.Move;
        }
        else
        {
            state = State.Idle;
        }
    }
}
