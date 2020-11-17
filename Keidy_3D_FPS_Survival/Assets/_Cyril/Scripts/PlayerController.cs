using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float applySpeed;
    [SerializeField]
    private float crouchSpeed;


    [SerializeField]
    private float jumpForce;

    //상태변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //앉았을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    private CapsuleCollider capsuleCollider;

    //카메라 민감도
    [SerializeField]
    private float lookSensitivity;

    //카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private Camera playerCamera;
    private Rigidbody myRigid;

    // Start is called before the first frame update
    void Start()
    { 
        //초기화
        myRigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        applySpeed = walkSpeed;

        originPosY = playerCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryRun();
        TryJump();
        TryCrouch();
        Move();
        CameraRotation();
        CharacterRotation();
    }

    //앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //앉기
    private void Crouch()
    {
        //앉기가 true면 false를, false면 true를 대입
        isCrouch = !isCrouch;

        //앉기 상태면 앉기 스피드로 변경 + 카메라 낮춤
        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        //일어서면 걷기 스피드로 돌아옴 + 카메라 머리쪽으로 이동
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        //playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, applyCrouchPosY, playerCamera.transform.localPosition.z);
        StartCoroutine(CrouchCoroutine());
    }

    //앉기 코루틴 (카메라가 부드럽게 움직이게)
    IEnumerator CrouchCoroutine()
    {
        float posY = playerCamera.transform.localPosition.y;
        int count = 0;

        while (posY != applyCrouchPosY)
        {
            count++;
            //카메라 위치를 부드럽게 (곡선 그래프처럼) 움직임
            posY = Mathf.Lerp(posY, applyCrouchPosY, 0.3f);
            playerCamera.transform.localPosition = new Vector3(0f, posY, 0f);

            if (count > 15) 
                break;
            yield return null;      //wait for 1 frame (wait untill next update invokes)
        }
        playerCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    //땅에 닿았는지 판정
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    //점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            jump();
        }
    }

    //점프
    private void jump()
    {
        //앉은 상태에서 점프하면 일어남
        if (isCrouch)
            Crouch();
        //앉은 상태가 아니라면 점프됨
        else
            myRigid.velocity = transform.up * jumpForce;
    }

    //달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    //달리기
    private void Running()
    {
        //앉은 상태에서 달리면 일어남
        if (isCrouch)
            Crouch();

        isRun = true;
        applySpeed = runSpeed;
    }

    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    //화면 좌우 회전
    private void CharacterRotation()
    {
        //좌우 캐릭터 회전
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;

        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));
    }

    //화면 상하 회전
    private void CameraRotation()
    {
        //상하 카메라 회전
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = -xRotation * lookSensitivity;
        currentCameraRotationX += cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        playerCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    //캐릭터 움직임
    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }
}
