using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool _hasFollowTarget; // FollowTarget이 설정되었는지 여부를 나타내는 플래그
    private Transform _followTarget; // 카메라가 따라가야 할 타겟 오브젝트

    // FollowTarget 프로퍼티: 타겟 설정 시 플래그 업데이트
    public Transform FollowTarget
    {
        get => _followTarget; // 타겟 반환
        set
        {
            _followTarget = value; // 새로운 타겟 설정
            _hasFollowTarget = _followTarget != null; // 타겟이 null인지 여부로 플래그 설정
        }
    }

    private void LateUpdate()
    {
        // 타겟이 설정된 경우에만 Transform 업데이트
        if (_hasFollowTarget)
        {
            SetTransform();
        }
    }

    // 카메라의 위치와 회전을 타겟에 맞게 동기화
    private void SetTransform()
    {
        _followTarget.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
