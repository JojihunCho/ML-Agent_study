using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody characterRigidbody;
    public int deleted = 0;

    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
	{
        if (ObjectSetting.level_start)
		{
            unitUpdate();
        }
	}

    void unitUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        // -1 ~ 1

        Vector3 velocity = new Vector3(inputX, 0, inputZ);
        
        if(velocity.x + velocity.z > 1.0)
		{
            velocity = velocity.normalized;
		}

        velocity *= speed;
        characterRigidbody.velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        //실행문
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            deleted = deleted + 1;
        }

        if (collision.gameObject.tag == "Key")
		{
            Debug.Log(deleted);
            Destroy(collision.gameObject);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        //실행문
    }

}
