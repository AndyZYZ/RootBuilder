using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ViewControl : MonoBehaviour
{
    CinemachineTargetGroup _targetGroup;
    private void Start()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();

    }

    private void Update()
    {
        if (GameManager.Instance.Score < 18)
            _targetGroup.m_Targets[1].radius = 7;
        else
            _targetGroup.m_Targets[1].radius = 11;
    }
}
