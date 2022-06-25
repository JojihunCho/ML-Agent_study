using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovig : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody characterRigidbody;
    public int deleted = 0;
    // Start is called before the first frame update
    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ObjectSetting.level_start)
        {
            unitUpdate();
        }
    }

    void unitUpdate()
	{

	}
}
