using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

public class Radar : MonoBehaviour {

    [SerializeField] private LayerMask radarLayerMask;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Transform sweepTransform;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float radarDistance = 150f;
    
    [SerializeField] private RadarPing radarPingPrefab;
    
    private List<Collider2D> _colliderList;

    private void Awake() 
    {
        _colliderList = new List<Collider2D>();
    }

    private void Update() 
    {
        transform.position = playerTransform.position;
        var previousRotation = (sweepTransform.eulerAngles.z % 360) - 180;
        
        sweepTransform.eulerAngles -= new Vector3(0, 0, rotationSpeed * Time.deltaTime);
        var currentRotation = (sweepTransform.eulerAngles.z % 360) - 180;

        if (previousRotation < 0 && currentRotation >= 0) 
        {
            // Half rotation
            _colliderList.Clear();
        }

        var radarHits = Physics2D.RaycastAll(transform.position, 
            GetVectorFromAngle(sweepTransform.eulerAngles.z), radarDistance, radarLayerMask);
        
        foreach (RaycastHit2D raycastHit2D in radarHits)
        {
            if (!raycastHit2D.collider) 
                continue;
            
            if (_colliderList.Contains(raycastHit2D.collider)) 
                continue;
            
            _colliderList.Add(raycastHit2D.collider);
            raycastHit2D.collider.TryGetComponent(typeof(IRadarTarget), out var target);
            
            if (target is IRadarTarget radarTarget)
            {
                var targetSpriteRenderer = radarTarget.GetSpriteRenderer();
                var ping = Instantiate(radarPingPrefab, target.transform.position, target.transform.rotation);
                ping.GetComponent<RadarPing>().Initialize(targetSpriteRenderer);
            }
        }
    }

    private static Vector3 GetVectorFromAngle(float angle) 
    {
        // angle = 0 -> 360
        var angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

}
