using Interfaces;
using UnityEngine;

public class VentHatch : MonoBehaviour, IObjectInfo
{
    private void Start()
    {
        var playerColliders = GameObject.FindGameObjectWithTag("Player").GetComponents<Collider2D>();
        
        if (playerColliders != null)
        {
            foreach (var playerCollider in playerColliders)
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
        }
        else
        {
            Debug.Log("Player collider is null");
        }
    }

    public string GetInfo()
    {
        return "Vent Hatch\nCan be used for bot movement";
    }
}