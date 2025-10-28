using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorySystem : MonoBehaviour
{
    [SerializeField] private float sensePerSecond = 50;
    private float _senseRate;

    [SerializeField] private float senseDistance = 10.0f;
    [SerializeField] private LayerMask desiredSensingLayers;

    private List<GameObject> _sensedObjects;
    public List<GameObject> SensedObjects => _sensedObjects;

    private Coroutine sensingCoroutine;
    
    void Start()
    {
        _senseRate = 1.0f / sensePerSecond;
        sensingCoroutine = StartCoroutine(Sense());
    }

    private IEnumerator Sense()
    {
        while (true)
        {
            _sensedObjects = Utilities.GetObjectsInRadius(transform.position, senseDistance, desiredSensingLayers);
            yield return new WaitForSeconds(_senseRate);
        }

        yield break;
    }
    
    private void FixedUpdate()
    {
        // cada fixed update
    }
}
