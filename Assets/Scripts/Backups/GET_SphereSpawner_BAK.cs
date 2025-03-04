using UnityEngine;
using System.Collections.Generic;

public class GET_SphereSpawner1 : MonoBehaviour
{
    public int numberOfSpheres = 100;
    public float areaSize = 10f;
    public GameObject spherePrefab;
    private List<Sphere> spheres = new List<Sphere>();

    void Start()
    {
        SpawnSpheres();
    }

    void FixedUpdate()
    {
        MoveSpheres();
    }

    void SpawnSpheres()
    {
        for (int i = 0; i < numberOfSpheres; i++)
        {
            int attempts = 0;
            bool placed = false;
            
            while (!placed && attempts < 100)
            {
                float baseSize = 1f;
                float scaleFactor = Random.Range(0.9f, 1.1f);
                float radius = (baseSize * scaleFactor) / 2;
                Vector3 randomPosition = new Vector3(
                    Random.Range(radius, areaSize - radius),
                    Random.Range(radius, areaSize - radius),
                    Random.Range(radius, areaSize - radius)
                );
                
                if (!IsOverlapping(randomPosition, radius))
                {
                    GameObject newSphere = Instantiate(spherePrefab, randomPosition, Quaternion.identity);
                    newSphere.transform.localScale = Vector3.one * baseSize * scaleFactor;
                    Rigidbody rb = newSphere.AddComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.mass = radius;
                    rb.linearDamping = 0.5f;
                    rb.angularDamping = 0.5f;
                    rb.linearVelocity = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                    Sphere sphere = new Sphere(newSphere, radius, rb);
                    spheres.Add(sphere);
                    placed = true;
                }
                attempts++;
            }
        }
    }

    void MoveSpheres()
    {
        foreach (Sphere sphere in spheres)
        {
            Vector3 randomForce = new Vector3(
                Random.Range(-0.02f, 0.02f),
                Random.Range(-0.02f, 0.02f),
                Random.Range(-0.02f, 0.02f)
            );
            sphere.rb.AddForce(randomForce, ForceMode.VelocityChange);
        }
    }

    bool IsOverlapping(Vector3 position, float radius)
    {
        foreach (Sphere sphere in spheres)
        {
            float distance = Vector3.Distance(position, sphere.position);
            if (distance < (radius + sphere.radius))
            {
                return true;
            }
        }
        return false;
    }

    private class Sphere
    {
        public GameObject gameObject;
        public float radius;
        public Rigidbody rb;
        
        public Vector3 position => gameObject.transform.position;
        
        public Sphere(GameObject obj, float r, Rigidbody rigidbody)
        {
            gameObject = obj;
            radius = r;
            rb = rigidbody;
        }
    }
}