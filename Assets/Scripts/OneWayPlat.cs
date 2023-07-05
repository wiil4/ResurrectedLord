using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlat : MonoBehaviour
{
    [SerializeField] LayerMask _detectPlayerLayer;      //PlayerLayer detector
    [SerializeField] LayerMask _undetectPlayerLayer;    //PlayerLayer undetector
    [SerializeField] private float _detectorTime = .25f;
    PlatformEffector2D _effector;
    // Start is called before the first frame update
    void Start()
    {
        _effector = GetComponent<PlatformEffector2D>();
    }

    public void UndetectPlayerTemporary()
    {
        StartCoroutine(IChangeLayer());     //StartCoroutine
    }

    IEnumerator IChangeLayer()
    {
        _effector.colliderMask = _undetectPlayerLayer;      //Change Layer Detection to Undetect
        yield return new WaitForSeconds(_detectorTime);             //Wait for seconds
        _effector.colliderMask = _detectPlayerLayer;        //Change Layer Detection to Detect
        StopCoroutine(IChangeLayer());
    }
}
