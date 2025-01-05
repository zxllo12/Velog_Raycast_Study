using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Player _player; // 플레이어
    [SerializeField] Camera _mainCamera; // 플레이어의 메인 카메라

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _lineDuration = 0.1f; // 선 표시 시간

    [Header("총 설정")]
    [SerializeField] Transform _muzzlePoint; // 총구 위치
    [SerializeField] int _bullet; // 현재 총알 수
    [SerializeField] int _maxBullet = 6; // 최대 총알 수
    [SerializeField] TextMeshProUGUI _bulletText; // 총알 표시 UI
    [SerializeField] GameObject _hitEffectPrefab; // 총알 자국

    Coroutine _reloadRoutine; // 재장전 코루틴
    Coroutine _shootRoutine; // 발사 코루틴

    private void Start()
    {
        // 라인 렌더러 기본값 초기화
        if (_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>(); // Line Renderer 추가
            _lineRenderer.positionCount = 2; // 두 점 지정
            _lineRenderer.startWidth = 0.05f; // 선의 시작 두께
            _lineRenderer.endWidth = 0.05f;   // 선의 끝 두께
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 기본 재질
            _lineRenderer.startColor = Color.blue; // 선의 시작 색
            _lineRenderer.endColor = Color.blue;   // 선의 끝 색
            _lineRenderer.enabled = false; // 기본적으로 비활성화
        }

        _bullet = _maxBullet;
        UpdateBulletText();
    }

    private void Update()
    {
        // 총구의 Ray 시각화 Scene View에서만 보이는 디버그용 선
        Debug.DrawRay(_muzzlePoint.position, _muzzlePoint.forward * 30f, Color.red);

        // 발사
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        // 재장전
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }       
    }

    private void Fire()
    {
        // 마우스가 잠금 상태가 아니면 발사하지 않음
        if (Cursor.lockState != CursorLockMode.Locked) 
            return;

        // 총알이 없으면 발사 불가
        if (_bullet <= 0)
        {
            UpdateBulletText();
            return;
        }

        // 발사 중이나 재장전 중이 아닐 때만 발사
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
        yield return new WaitForSeconds(2.3f); // 재장전 시간
        _bullet = _maxBullet; // 총알 갱신
        UpdateBulletText();
        _reloadRoutine = null;
    }

    private IEnumerator ShootRoutine()
    {
        _bullet--; // 총알 감소
        UpdateBulletText();

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"{hit.transform.name}에 명중!");

            // 라인 렌더러로 선 표시
            if (_lineRenderer != null)
            {
                _lineRenderer.SetPosition(0, _muzzlePoint.position); // 시작점
                _lineRenderer.SetPosition(1, hit.point); // 끝점
                _lineRenderer.enabled = true;

                yield return new WaitForSeconds(_lineDuration); // 일정 시간 후 비활성화
                _lineRenderer.enabled = false;
            }

            // 이펙트 생성
            if (_hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitEffect, 3f);
            }
        }

        yield return new WaitForSeconds(0.5f); // 발사 지연
        _shootRoutine = null; 
    }

    // 총알 UI 업데이트
    private void UpdateBulletText()
    {
        _bulletText.text = $"{_bullet} / {_maxBullet}";
    }
}
