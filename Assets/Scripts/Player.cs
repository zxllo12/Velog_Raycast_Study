using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    Rigidbody _rb; // �÷��̾��� Rigidbody ������Ʈ

    [Header("�÷��̾� ������")]
    [SerializeField] float _moveSpeed = 3f; // �⺻ �̵� �ӵ�
    [SerializeField] float _rotateSpeed = 300f; // ȸ�� �ӵ�
    [SerializeField] float _jumpForce = 10; // ���� �� ����Ǵ� ��
    [SerializeField] float _mouseSensitivity = 1f; // ���콺 �ΰ���
    float _run; // �޸��� �ӵ�

    [Header("�÷��̾� ī�޶�")]
    [SerializeField] Vector3 _offset; // ī�޶�� �÷��̾� �� �Ÿ� ������
    [SerializeField] float _yRotationRange = 60f; // ī�޶��� ���� ȸ�� ���� ����
    [SerializeField] Camera _mainCamera; // �÷��̾��� ���� ī�޶�
    [SerializeField] bool _isJumped; // �÷��̾ ���� ������ Ȯ���ϴ� ����
    [SerializeField] float _yRotation = 0f; // ī�޶��� ���� ���� ȸ�� ����

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();

        _isJumped = false; // �ʱ� ���� ���� ����

        _run = _moveSpeed * 2; // �޸��� �ӵ��� �⺻ �̵� �ӵ��� �� ��� ����

        // Ŀ�� ����� �� ���
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // ���� Shift Ű�� ���� �޸���, ���� �⺻ �ӵ��� ����
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
            _isJumped = true; // ���� ���·� ����
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse); // ���� ���� ���� ����
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
        // ���콺 �Է°� ��������
        Vector2 rotateInput = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        SetRotation(rotateInput); // �÷��̾�� ī�޶� ȸ�� ó��

        // Ű���� �Է°� ��������
        Vector3 moveInput = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        SetPosition(moveInput); // �÷��̾� �̵� ó��
    }

    private void SetRotation(Vector2 input)
    {
        // ���콺 X �Է����� �÷��̾� �¿� ȸ��
        if (input.x != 0)
        {
            transform.Rotate(Vector3.up, input.x * _rotateSpeed * _mouseSensitivity * Time.deltaTime);
        }

        // ���콺 Y �Է����� ī�޶� ���� ȸ��
        if (input.y != 0)
        {
            _yRotation = _yRotation + -input.y * _rotateSpeed * _mouseSensitivity * Time.deltaTime; // ���� ȸ�� ���
            _yRotation = Mathf.Clamp(_yRotation, -_yRotationRange, _yRotationRange); // ȸ�� ������ ����

            _mainCamera.transform.localRotation = Quaternion.Euler(_yRotation, 0f, 0f); // ī�޶� ȸ�� ����
        }
    }

    private void SetPosition(Vector3 input)
    {
        // �Է°����� �̵� ���� ���
        Vector3 moveDirection = transform.forward * input.z + transform.right * input.x;
        moveDirection.Normalize(); // ���� ���� ����ȭ
        Vector3 velocity = moveDirection * _moveSpeed; // �̵� �ӵ� ���

        // �ε巯�� �̵��� ���� �ӵ� ���� ó��
        _rb.velocity = Vector3.Lerp(_rb.velocity, new Vector3(velocity.x, _rb.velocity.y, velocity.z), 0.1f);
    }
}

