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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ObjectSetting.level_start = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ObjectSetting.level_start = false;
        }
    }

	public void reset()
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
            case 2:
                delete_child();
                level_2();
                ObjectSetting.first = false;
                break;
            default:
                delete_child();
                level_3();
                ObjectSetting.first = false;
                break;
        }
    }

    public void level_0()
	{
        for(int i = 0; i < enemyInHo; i++)
		{
            GameObject myInstance = Instantiate(enemy, new Vector3(-10f + i * (20f / (enemyInHo - 1)), 1f, 0f), Quaternion.identity) as GameObject;
            myInstance.transform.parent = parent;
            
        }
	}

    public void level_1()
    {
        for (int i = 0; i < enemyInHo; i++)
        {
            float zdata = Random.Range(-10,10);
            GameObject myInstance = Instantiate(enemy, new Vector3(-10f + i * (20f / (enemyInHo - 1)), 1f, zdata), Quaternion.identity) as GameObject;
            myInstance.transform.parent = parent;

        }
    }

    public void level_2()
    {
        for (int i = 0; i < enemyInHo - 2; i++)
        {
            for(int j = 0; j < enemyInVer - 2; j++)
			{
                GameObject myInstance = Instantiate(enemy, new Vector3(-10f + i * (20f / (enemyInHo - 3)), 1f, -10f + j * (20 / (enemyInVer - 3))), Quaternion.identity) as GameObject;
                myInstance.transform.parent = parent;
            }

        }
    }

    public void level_3()
    {
        for (int i = 0; i < enemyInHo; i++)
        {
            float zdata = Random.Range(-10, 10);
            bool randBool = (Random.value > 0.5f);
            GameObject myInstance = Instantiate(enemy, new Vector3(-10f + i * (20f / (enemyInHo - 1)), 1f, 0f), Quaternion.identity) as GameObject;
            myInstance.transform.parent = parent;
            if(randBool) myInstance.GetComponent<EnemyMovig>().setVelocity(new Vector3(1, 0, 1));
            else myInstance.GetComponent<EnemyMovig>().setVelocity(new Vector3(1, 0, -1));
        }
    }

    public void delete_child()
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
