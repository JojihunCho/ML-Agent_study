using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMoving : MonoBehaviour
{
    public float speed = 4.0f;
    private Rigidbody characterRigidbody;

    public Vector3 preVer;

    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        unitUpdate();
    }
    void unitUpdate()
    {
        preVer = characterRigidbody.velocity;
        Vector3 temp = preVer;
        temp.y = 0;
        temp.Normalize();
        characterRigidbody.velocity = /*new Vector3(0f, preVer.y, 0f) +*/ temp * speed;
    }
    public void setVelocity(Vector3 _velocity)
    {
        characterRigidbody = GetComponent<Rigidbody>();
        _velocity.Normalize();
        characterRigidbody.velocity = _velocity * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("tile"))
        characterRigidbody.velocity = calculRefc(preVer, -collision.GetContact(0).normal);
        Debug.Log(collision);
    }


    Vector3 calculRefc(Vector3 a, Vector3 n)
    {
        Vector3 p = -Vector3.Dot(a, n) / n.magnitude * n / n.magnitude;
        Vector3 b = a + 2 * p;
        return b;
    }
}
