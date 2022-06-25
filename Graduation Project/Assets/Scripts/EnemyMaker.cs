using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMaker : MonoBehaviour
{
    public GameObject enemy;
    public int enemyInHo = 7;
    public int enemyInVer = 7;
    public int enemySpeed = 7;
    private Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(ObjectSetting.level_start == false)
		{
            switch (ObjectSetting.level)
			{
                case 0:
                    delete_child();
                    level_0();
                    ObjectSetting.first = false;
                    //level = -1;
                    break;
                case 1:
                    delete_child();
                    level_1();
                    ObjectSetting.first = false;
                    break;
                default:
                    delete_child();
                    level_2();
                    ObjectSetting.first = false;
                    break;
            }
		}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ObjectSetting.level_start = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ObjectSetting.level_start = false;
        }
    }

    void level_0()
	{
        for(int i = 0; i < enemyInHo; i++)
		{
            GameObject myInstance = Instantiate(enemy, new Vector3(-10f + i * (20f / (enemyInHo - 1)), 1f, 0f), Quaternion.identity) as GameObject;
            myInstance.transform.parent = parent;
            
        }
	}

    void level_1()
    {
        for (int i = 0; i < enemyInHo; i++)
        {
            float zdata = Random.Range(-10,10);
            GameObject myInstance = Instantiate(enemy, new Vector3(-10f + i * (20f / (enemyInHo - 1)), 1f, zdata), Quaternion.identity) as GameObject;
            myInstance.transform.parent = parent;

        }
    }

    void level_2()
    {
        for (int i = 0; i < enemyInHo; i++)
        {
            for(int j = 0; j < enemyInVer; j++)
			{
                GameObject myInstance = Instantiate(enemy, new Vector3(-10f + i * (20f / (enemyInHo - 1)), 1f, -10f + j * (20 / (enemyInVer - 1))), Quaternion.identity) as GameObject;
                myInstance.transform.parent = parent;
            }

        }
    }

    void delete_child()
	{
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            // 자기 자신의 경우엔 무시 
            // (게임오브젝트명이 다 다르다고 가정했을 때 통하는 코드)
            if (child.name == transform.name)
                continue;
            Destroy(child.gameObject);
        }
    }
}
