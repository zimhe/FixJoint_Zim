using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpatialSlur.SlurUnity;

/*
 * Notes
 */ 

public class Follow : MonoBehaviour
{
    public GameObject  Target;
    
    [Range(1.0f, 10.0f)]
    public float Stiffness = 5.0f;

    private Vector3  _target;


    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
       
    }


    // Update is called once per frame
    void Update()
    {
        _target = Target.GetComponent<Transform>().position;


        if (_target != null)
            transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * Stiffness);
    }
}
