using UnityEngine;

public class IgnorePlayerCollision : MonoBehaviour
{
    private void Start()
    {
        var playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        
        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
        }
        else
        {
            Debug.Log("Player collider is null");
        }
    }
}