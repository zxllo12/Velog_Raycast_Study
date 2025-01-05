using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool _hasFollowTarget; // FollowTarget�� �����Ǿ����� ���θ� ��Ÿ���� �÷���
    private Transform _followTarget; // ī�޶� ���󰡾� �� Ÿ�� ������Ʈ

    // FollowTarget ������Ƽ: Ÿ�� ���� �� �÷��� ������Ʈ
    public Transform FollowTarget
    {
        get => _followTarget; // Ÿ�� ��ȯ
        set
        {
            _followTarget = value; // ���ο� Ÿ�� ����
            _hasFollowTarget = _followTarget != null; // Ÿ���� null���� ���η� �÷��� ����
        }
    }

    private void LateUpdate()
    {
        // Ÿ���� ������ ��쿡�� Transform ������Ʈ
        if (_hasFollowTarget)
        {
            SetTransform();
        }
    }

    // ī�޶��� ��ġ�� ȸ���� Ÿ�ٿ� �°� ����ȭ
    private void SetTransform()
    {
        _followTarget.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
