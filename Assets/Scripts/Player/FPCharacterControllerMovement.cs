using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCharacterControllerMovement : MonoBehaviour
{
    //
    public float Gravity;
    public float JumpHeight;

    public float WalkSpeed;
    public float SprintingSpeed;
    public float WalkSpeedCrouched;
    public float SprintingSpeedCrouched;
    public float CrouchHeight;


    private CharacterController characterController;
    private Transform characterTransform;
    private Vector3 MovementDirection;
    private bool isCrouched;
    private float OriginHeight;
    [SerializeField] private Animator characterAnimator;
    public Animator tpCharacterAnimator;
    private float velocity;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterTransform = transform;
        OriginHeight = characterController.height;
        Cursor.lockState = CursorLockMode.Locked;//锁定指针到视图中心
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 暂停
        if (GlobleVar.isPause)
        {
            return;
        }
        float tmp_CurrentSpeed = WalkSpeed;
        if (characterController.isGrounded)
        {

            // 获取输入
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");

            // 移动方向
            MovementDirection = characterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical));

            // 跳跃
            if (Input.GetButtonDown("Jump"))
            {
                MovementDirection.y = JumpHeight;
            }

            // 奔跑
            // 下蹲跑
            if (isCrouched)
            {
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeedCrouched : WalkSpeedCrouched;
            }
            else
            {
                tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintingSpeed : WalkSpeed;
            }

            // 下蹲控制
            if (Input.GetKeyDown(KeyCode.C))
            {
                var tmp_CurrentHeight = isCrouched ? OriginHeight : CrouchHeight;
                StartCoroutine(DoCrouch(tmp_CurrentHeight));
                isCrouched = !isCrouched;
            }

            if (characterAnimator != null)
            {
                // 动画控制，通过velocity控制站立，行走，奔跑的手部动画
                var tmp_Velocity = characterController.velocity;
                velocity = new Vector3(tmp_Velocity.x, 0, tmp_Velocity.z).magnitude;
                characterAnimator.SetFloat("Velocity", velocity, 0.25f, Time.deltaTime);

            }
            if (tpCharacterAnimator != null)
            {
                // 身体动画控制
                tpCharacterAnimator.SetFloat("Velocity", velocity, 0.25f, Time.deltaTime);
                tpCharacterAnimator.SetFloat("Movement_X", tmp_Horizontal, 0.25f, Time.deltaTime);
                tpCharacterAnimator.SetFloat("Movement_Y", tmp_Vertical, 0.25f, Time.deltaTime);
            }
        }

        // 重力下落
        MovementDirection.y -= Gravity * Time.deltaTime;
        characterController.Move(tmp_CurrentSpeed * Time.deltaTime * MovementDirection.normalized);
        //Debug.Log(characterController.velocity.magnitude);
    }

    // 下蹲过程
    private IEnumerator DoCrouch(float target)
    {
        float tmp_CurrentVelocity = 0;
        while (Mathf.Abs(characterController.height - target) > 0.1f)
        {
            yield return null;
            characterController.height = Mathf.SmoothDamp(characterController.height, 
                target, 
                ref tmp_CurrentVelocity, 
                Time.deltaTime * 5);
        }
    }

    public void SetupAnimator(Animator animator)
    {
        characterAnimator = animator;
    }
}
