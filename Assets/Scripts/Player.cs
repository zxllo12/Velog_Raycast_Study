using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    Rigidbody _rb; // 플레이어의 Rigidbody 컴포넌트

    [Header("플레이어 움직임")]
    [SerializeField] float _moveSpeed = 3f; // 기본 이동 속도
    [SerializeField] float _rotateSpeed = 300f; // 회전 속도
    [SerializeField] float _jumpForce = 10; // 점프 시 적용되는 힘
    [SerializeField] float _mouseSensitivity = 1f; // 마우스 민감도
    float _run; // 달리기 속도

    [Header("플레이어 카메라")]
    [SerializeField] Vector3 _offset; // 카메라와 플레이어 간 거리 오프셋
    [SerializeField] float _yRotationRange = 60f; // 카메라의 상하 회전 제한 각도
    [SerializeField] Camera _mainCamera; // 플레이어의 메인 카메라
    [SerializeField] bool _isJumped; // 플레이어가 점프 중인지 확인하는 변수
    [SerializeField] float _yRotation = 0f; // 카메라의 현재 상하 회전 각도

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();

        _isJumped = false; // 초기 점프 상태 설정

        _run = _moveSpeed * 2; // 달리기 속도를 기본 이동 속도의 두 배로 설정

        // 커서 숨기기 및 잠금
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 왼쪽 Shift 키를 눌러 달리기, 떼면 기본 속도로 복구
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            _moveSpeed = Input.GetKey(KeyCode.LeftShift) ? _run : _run / 2;
        }

        Jump();
        Move();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isJumped)
        {
            _isJumped = true; // 점프 상태로 변경
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // 위로 힘을 가해 점프
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _isJumped = false;
        }
    }

    private void Move()
    {
        // 마우스 입력값 가져오기
        Vector2 rotateInput = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        SetRotation(rotateInput); // 플레이어와 카메라 회전 처리

        // 키보드 입력값 가져오기
        Vector3 moveInput = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        SetPosition(moveInput); // 플레이어 이동 처리
    }

    private void SetRotation(Vector2 input)
    {
        // 마우스 X 입력으로 플레이어 좌우 회전
        if (input.x != 0)
        {
            transform.Rotate(Vector3.up, input.x * _rotateSpeed * _mouseSensitivity * Time.deltaTime);
        }

        // 마우스 Y 입력으로 카메라 상하 회전
        if (input.y != 0)
        {
            _yRotation = _yRotation + -input.y * _rotateSpeed * _mouseSensitivity * Time.deltaTime; // 상하 회전 계산
            _yRotation = Mathf.Clamp(_yRotation, -_yRotationRange, _yRotationRange); // 회전 각도를 제한

            _mainCamera.transform.localRotation = Quaternion.Euler(_yRotation, 0f, 0f); // 카메라 회전 적용
        }
    }

    private void SetPosition(Vector3 input)
    {
        // 입력값으로 이동 방향 계산
        Vector3 moveDirection = transform.forward * input.z + transform.right * input.x;
        moveDirection.Normalize(); // 방향 벡터 정규화
        Vector3 velocity = moveDirection * _moveSpeed; // 이동 속도 계산

        // 부드러운 이동을 위해 속도 보간 처리
        _rb.velocity = Vector3.Lerp(_rb.velocity, new Vector3(velocity.x, _rb.velocity.y, velocity.z), 0.1f);
    }
}

