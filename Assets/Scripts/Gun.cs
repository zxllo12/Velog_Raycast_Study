using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Player _player; // �÷��̾�
    [SerializeField] Camera _mainCamera; // �÷��̾��� ���� ī�޶�

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _lineDuration = 0.1f; // �� ǥ�� �ð�

    [Header("�� ����")]
    [SerializeField] Transform _muzzlePoint; // �ѱ� ��ġ
    [SerializeField] int _bullet; // ���� �Ѿ� ��
    [SerializeField] int _maxBullet = 6; // �ִ� �Ѿ� ��
    [SerializeField] TextMeshProUGUI _bulletText; // �Ѿ� ǥ�� UI
    [SerializeField] GameObject _hitEffectPrefab; // �Ѿ� �ڱ�

    Coroutine _reloadRoutine; // ������ �ڷ�ƾ
    Coroutine _shootRoutine; // �߻� �ڷ�ƾ

    private void Start()
    {
        // ���� ������ �⺻�� �ʱ�ȭ
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>(); // Line Renderer �߰�
            _lineRenderer.positionCount = 2; // �� �� ����
            _lineRenderer.startWidth = 0.05f; // ���� ���� �β�
            _lineRenderer.endWidth = 0.05f;   // ���� �� �β�
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // �⺻ ����
            _lineRenderer.startColor = Color.blue; // ���� ���� ��
            _lineRenderer.endColor = Color.blue;   // ���� �� ��
            _lineRenderer.enabled = false; // �⺻������ ��Ȱ��ȭ
        }

        _bullet = _maxBullet;
        UpdateBulletText();
    }

    private void Update()
    {
        // �ѱ��� Ray �ð�ȭ Scene View������ ���̴� ����׿� ��
        Debug.DrawRay(_muzzlePoint.position, _muzzlePoint.forward * 30f, Color.red);

        // �߻�
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        // ������
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }       
    }

    private void Fire()
    {
        // ���콺�� ��� ���°� �ƴϸ� �߻����� ����
        if (Cursor.lockState != CursorLockMode.Locked) 
            return;

        // �Ѿ��� ������ �߻� �Ұ�
        if (_bullet <= 0)
        {
            UpdateBulletText();
            return;
        }

        // �߻� ���̳� ������ ���� �ƴ� ���� �߻�
        if (_shootRoutine == null && _reloadRoutine == null)
        {
            _shootRoutine = StartCoroutine(ShootRoutine());
        }
    }

    private void Reload()
    {
        if (_reloadRoutine == null)
        {
            _reloadRoutine = StartCoroutine(ReloadGunRoutine());
        }
    }

    private IEnumerator ReloadGunRoutine()
    {
        yield return new WaitForSeconds(2.3f); // ������ �ð�
        _bullet = _maxBullet; // �Ѿ� ����
        UpdateBulletText();
        _reloadRoutine = null;
    }

    private IEnumerator ShootRoutine()
    {
        _bullet--; // �Ѿ� ����
        UpdateBulletText();

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"{hit.transform.name}�� ����!");

            // ���� �������� �� ǥ��
            if (_lineRenderer != null)
            {
                _lineRenderer.SetPosition(0, _muzzlePoint.position); // ������
                _lineRenderer.SetPosition(1, hit.point); // ����
                _lineRenderer.enabled = true;

                yield return new WaitForSeconds(_lineDuration); // ���� �ð� �� ��Ȱ��ȭ
                _lineRenderer.enabled = false;
            }

            // ����Ʈ ����
            if (_hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 3f);
            }
        }

        yield return new WaitForSeconds(0.5f); // �߻� ����
        _shootRoutine = null; 
    }

    // �Ѿ� UI ������Ʈ
    private void UpdateBulletText()
    {
        _bulletText.text = $"{_bullet} / {_maxBullet}";
    }
}
