using UnityEngine;

public class CollisionDebugger : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        Debug.Log("Object Tag: " + collision.gameObject.tag);
    }
}
