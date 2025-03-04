using UnityEngine;

public class GET_Raycast : MonoBehaviour
{
    public float range = 7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Raycast script has started");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.forward;
        Ray playerRay = new Ray(transform.position, transform.TransformDirection(direction * range));
        Debug.DrawRay(transform.position, transform.TransformDirection(direction * range), Color.red);

        if (Physics.Raycast(playerRay, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Static"))
            {
                print("My raycast hit a STATIC object");
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                print("My raycast hit an ENEMY object");

                // New functionality: Change the hit object's color to a random color
                Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
                if (hitRenderer != null) // Ensure the object has a Renderer component
                {
                hitRenderer.material.color = GetRandomColor();
                }
            }
        }
    }

    // Helper function to generate a random color
    Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
