using System.Collections;
using UnityEngine;

public class CameraRelativeMovement : MonoBehaviour
{
    public float speed = 6f;
    public float turnSmoothTime = 0.1f; // 旋转平滑时间
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float turnSmoothVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        StartCoroutine(enumerator());
    }

    void Update()
    {
        // 获取输入（-1 到 1）
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(horizontal, vertical).normalized;

        // 检测是否着地
        isGrounded = controller.isGrounded;
        if (isGrounded && current.y < 0)
        {
            current.y = -2f; // 防止粘地
        }

        if (input.magnitude >= 0.1f)
        {
            // 获取主相机的水平朝向（忽略Y轴）
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            Transform cam = Camera.main.transform;
            if (cam != null)
            {
                targetAngle += cam.eulerAngles.y; // 加上相机的偏航角
            }

            // 平滑旋转角色
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 计算移动方向（基于角色新朝向）
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // 应用移动
            move = moveDir * speed;
            //controller.Move(moveDir * speed * Time.deltaTime);
        }

        // 跳跃
        if (Input.GetButton("Jump") && isGrounded)
        {
            open = true;
            time = Time.time;
        }

        if(!isGrounded)
        {
            down = -Vector3.up * 10f;
            //controller.Move(-Vector3 .up*20f  * Time.deltaTime);
        }
        if(Time.time -time>2f)
        {
            open = false;
        }
        current = jump + move + down;
        controller.Move(current * Time.deltaTime);


    }
    bool open=false ;
    float time = 0.3f;
    Vector3 jump;
    Vector3 move;
    Vector3 down;
    Vector3 current;
    IEnumerator enumerator()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame ();
            if(open)
            {
                jump = Vector3.up * 20f;
                //controller.Move(Vector3.up * 50f * Time.deltaTime);
            }
        }
    }
}


