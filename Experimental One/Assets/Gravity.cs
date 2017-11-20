using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {
    [SerializeField] GameObject attractorA;
    [SerializeField] GameObject attractorB;

    [Range(0f,1000000000000f)][SerializeField] float mass;
    static float G = 6.674f * Mathf.Pow(10.0f, -11.0f);
    List<Rigidbody> neighbours;
    Rigidbody rb1;

    void Start()
    {
        rb1 = GetComponent<Rigidbody>();
        neighbours.Add(attractorA.GetComponent<Rigidbody>());
        neighbours.Add(attractorB.GetComponent<Rigidbody>());
        print(neighbours.ToArray());
    }

    private void FixedUpdate()
    {
        float force;
        rb1.mass = mass;
        float rb1mass = rb1.mass;
        Rigidbody rb2;
        float rb2mass;

        for (int c = 0; c < neighbours.Count; c++)
        {
            rb2 = neighbours[c];
            rb2mass = rb2.mass;
            float r = Vector3.Distance(transform.position, neighbours[c].transform.position);
            force = Gforce(rb1mass, rb2mass, r);
            var heading = transform.position - rb2.position;
            rb1.AddForce(force * heading);
            rb2.AddForce(force * heading);
        }
    }

    private static float Gforce(float m1, float m2, float r)
    {
        float F;
        F = G * ((m1 * m2) / Mathf.Pow(r, 2));
        return F;
    }

 /*   private void OnTriggerEnter(Collider collider)
    {
        var rb2 = collider.GetComponent<Rigidbody>();
        Debug.Log("neighbour check");
        if (neighbours.Contains(rb2)) return;
        else neighbours.Add(rb2);
        Debug.Log("new neighbour");
    }
    */
}
