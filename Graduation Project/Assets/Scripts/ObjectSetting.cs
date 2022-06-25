using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSetting : MonoBehaviour
{
    public GameObject player;
    public GameObject key;
    public GameObject enemyManager;
    public static bool level_start = false, first = true, reset = false;
    public static int level = 0;
    public Vector3 playerPos = Vector3.zero;
    public Vector3 keyPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        playerPos = new Vector3(13f, 1f, 13f);
        keyPos = new Vector3(-13f, 1f, -13f);
        Remove();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if(level_start == false || reset)
		{
            reset = false;
            Remove();
            Reset();
            GameObject.FindWithTag("GM").GetComponent<EnemyMaker>().reset();
        }


		

        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            level = 0; //정적 모델
            first = true;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            level = 1; //정적 모델
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            level = 2; //정적 모델
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            level = 3; //동적 모델
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            level = 4; //동적 모델
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            level = 5;
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            level = 6;
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            level = 7;
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            level = 8;
            first = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            level = 9; 
            first = true;
        }

    }

    void LastUpdate()
	{
        
    }

    void Remove()
	{
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            // 자기 자신의 경우엔 무시 
            // (게임오브젝트명이 다 다르다고 가정했을 때 통하는 코드)
            if (child.name == transform.name || child.name == "EnemyMaker")
                continue;
            Destroy(child.gameObject);
        }
    }

    void Reset()
	{
        GameObject keyInstance = Instantiate(key, keyPos, Quaternion.identity) as GameObject;
        keyInstance.transform.parent = this.transform;

        GameObject playInstance = Instantiate(player, playerPos, Quaternion.identity) as GameObject;
        playInstance.transform.parent = this.transform;
    }
}
