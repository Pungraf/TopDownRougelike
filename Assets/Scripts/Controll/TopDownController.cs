using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TopDownController : NetworkBehaviour
{
    private InputHandler _input;
    private Animator _animator;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    LayerMask _aimLayerMask;

    private void Awake()
    {
        _input = GetComponent<InputHandler>();
        _animator = GetComponentInChildren<Animator>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)
        {
            return;
        }
        var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
        //Moving
        MoveTowardTarget(targetVector);
        //Rotating
        RotateTowardMouseVector();
        //Animating
        float velozityX = Vector3.Dot(targetVector.normalized, transform.right);
        float velozityZ = Vector3.Dot(targetVector.normalized, transform.forward);

        _animator.SetFloat("VelocityX", velozityX, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityZ", velozityZ, 0.1f, Time.deltaTime);
    }

    private void RotateTowardMouseVector()
    {
        Ray ray = camera.ScreenPointToRay(_input.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f, _aimLayerMask))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }

    private void MoveTowardTarget(Vector3 targetVector)
    {
        var speed = moveSpeed * Time.deltaTime;

        targetVector = Quaternion.Euler(0, camera.gameObject.transform.eulerAngles.y, 0) * targetVector ;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
    }

}
