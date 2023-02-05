using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : MonoBehaviourSingleton<FlowerController>
{
    Animator _animator;

    private void Start()
    {
        _animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetInteger("Level", GameManager.Instance.GetLevel());
    }
}
