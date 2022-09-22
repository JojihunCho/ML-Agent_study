using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

public class PF_Last : Agent
{
    public GameObject Enemy;
    public Transform targetTr;
    public Renderer floorRd;
    public GameObject wall;
    public GameObject floor;
    public Material badMt;
    public Material goodMt;

    public float agentSpeed = 5.0f;
    
    public int num_ball = 1;
    [Range(10, 100)]
    public int map_size = 10; //max map size and default map size
    public bool randTargetPos = false;
    public bool randAgentPos = false;
    public bool randMapSize = false;

    public float time_reward = -0.1f, wall_enter = -5.0f, wall_stay = -1.0f,
    enemy_reward = -50.0f, distance_reward = 10.0f, target_reward = 100.0f;

    private Material originMt;
    private Transform tr;
    private Rigidbody rb;
    private List<GameObject> enemy = new List<GameObject>();
    private List<Transform> enemyTr = new List<Transform>();
    private List<Rigidbody> enemyRb = new List<Rigidbody>();
    private List<int> randList;
    

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < num_ball; i++)
        {
            enemy.Add(Instantiate(Enemy));
            enemyTr.Add(enemy[i].GetComponent<Transform>());
            enemyTr[i].parent = transform.parent;
            enemyRb.Add(enemy[i].GetComponent<Rigidbody>());
        }

        originMt = floorRd.material;
    }

    IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }

    public override void OnEpisodeBegin()
    {
        randList = new List<int>();
        var field = map_size - 1;
        if (randMapSize) field = changeMap(Random.Range(10, map_size));
        randInit(field);
        //물리력을 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        var baseCamp = field / 2;

        if (randAgentPos)   tr.localPosition = new Vector3(randList[0] / field - baseCamp, 0.65f, randList[0] % field - baseCamp);
        else    tr.localPosition = new Vector3(baseCamp, 0.65f, baseCamp);
        if (randTargetPos)  targetTr.localPosition = new Vector3(randList[1] / field - baseCamp, 0.65f, randList[1] % field - baseCamp);
        else    targetTr.localPosition = new Vector3(0.0f, 0.65f, 0.0f);
        targetTr.localRotation = Quaternion.identity;

        setBallsPos(field);

        StartCoroutine(RevertMaterial());
    }

    private int changeMap(int input)
    {
        floor.transform.localScale = new Vector3(input, 0.1f, input);
        wall.transform.localScale = new Vector3(input/10f, 1f, input/10f);
        return input - 1;
    }

    private void randInit(int input)
    {
        for (var i = 0; i < (input - 1) * (input - 1); i++) randList.Add(i);

        for (var i = 0; i < num_ball + 2; i++)
        {
            var temp = randList[i];
            var ran = Random.Range(0, randList.Count);
            randList[i] = randList[ran];
            randList[ran] = temp;
        }
    }

    private void setBallsPos(int field)
    {
        var useBallNum = num_ball * (field + 1) / map_size;
        var i = 2;
        var baseCamp = field / 2;
        for (i = 2; i < useBallNum + 2; i++)
        {
            enemyTr[i - 2].localPosition = new Vector3(randList[i] / field - baseCamp, 0.5f, randList[i] % field - baseCamp);
            enemyTr[i - 2].localRotation = Quaternion.identity;
            float randThetha = Random.Range(0, 361) * Mathf.PI / 180;
            enemy[i - 2].GetComponent<BallMoving>().setVelocity(new Vector3(Mathf.Sin(randThetha), 0, Mathf.Cos(randThetha)));
            enemyTr[i - 2].gameObject.SetActive(true);
        }
        for(;i < num_ball + 2; i++)
        {
            enemy[i - 2].SetActive(false);
        }
    }

    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition - tr.localPosition);  //3 (x,y,z)
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionX = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        var actionZ = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        Vector3 dir = (Vector3.forward * actionZ) + (Vector3.right * actionX);
        rb.velocity = (dir.normalized * agentSpeed);

        //지속적으로 이동을 이끌어내기 위한 마이너스 보상
        SetReward(time_reward);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("ball"))
        {
            floorRd.material = badMt;
            //잘못된 행동일 때 마이너스 보상을 준다.
            SetReward(enemy_reward);
            //종료시 타겟과 얼마나 가까운가
            float distence = Vector3.Magnitude(targetTr.localPosition - tr.localPosition);
            if (distence > 0.5f) SetReward(distance_reward / (2 * distence));
            else SetReward(distance_reward);
            //학습을 종료시키는 메소드
            EndEpisode();
        }

        if (coll.collider.CompareTag("wall"))
        {
            SetReward(wall_enter);
        }

        if (coll.collider.CompareTag("target"))
        {
            floorRd.material = goodMt;
            //올바른 행동일 때 플러스 보상을 준다.
            SetReward(target_reward);
            //학습을 종료시키는 메소드
            EndEpisode();
        }
    }

    void OnCollisionStay(Collision coll)
    {
        if (coll.collider.CompareTag("wall"))
        {
            SetReward(wall_stay);
        }
    }
}
