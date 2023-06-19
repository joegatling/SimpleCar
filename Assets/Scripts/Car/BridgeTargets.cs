using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTargets : MonoBehaviour
{
    [SerializeField] private Transform _targetA;
    [SerializeField] private Transform _targetB;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(_targetA.position, _targetB.position, 0.5f);
        transform.LookAt(_targetA);
        //transform.Rotate(transform.up, -90f);
    }
}
