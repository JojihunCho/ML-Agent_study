using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovig : MonoBehaviour
{
    public float speed = 7f;
    private Rigidbody characterRigidbody;

    public bool go_x = true;
    private Vector3 preVer;
    // Start is called before the first frame update
    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        characterRigidbody = GetComponent<Rigidbody>();
        if (ObjectSetting.level_start)
        {
            unitUpdate();
        }
    }

    public void setVelocity(Vector3 _velocity)
	{
        characterRigidbody = GetComponent<Rigidbody>();
        _velocity.Normalize();
        characterRigidbody.velocity = _velocity * speed;
    }

    public void setSpeed(float _speed)
    {
        speed = _speed;
    }

    void unitUpdate()
	{
        preVer = characterRigidbody.velocity;
        Vector3 temp = preVer;
        temp.y = 0;
        temp.Normalize();
        characterRigidbody.velocity = new Vector3(0f, preVer.y, 0f) + temp * speed;
    }

    Vector3 calculRefc(Vector3 a, Vector3 n)
	{
        Vector3 p = -Vector3.Dot(a,n)/n.magnitude * n/n.magnitude;
        Vector3 b = a + 2 * p;
        return b;
	}

    void OnCollisionEnter(Collision collision)
	{
        if(collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Key"))
		{
            characterRigidbody.velocity = calculRefc(preVer, -collision.GetContact(0).normal);
		}

    }
}
