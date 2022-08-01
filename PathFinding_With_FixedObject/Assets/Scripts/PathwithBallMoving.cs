using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

public class PathwithBallMoving : Agent
{
    public float speed = 5.0f;
    private Transform tr;
    private Rigidbody rb;
    public Transform targetTr;
    public int num_ball = 1;
    public List<GameObject> enemy = new List<GameObject>();
    private List<Transform> enemyTr = new List<Transform>();
    private List<Rigidbody> enemyRb = new List<Rigidbody>();

    //private int num_vector = 16;

    public Renderer floorRd;

    private Material originMt;
    public Material badMt;
    public Material goodMt;

    public float time_reward = -0.1f, wall_enter = -5.0f, wall_stay = -1.0f,
    enemy_reward = -50.0f, distance_reward = 10.0f, target_reward = 100.0f;
    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        for(int i = 0; i < num_ball; i++)
        {
            enemyTr.Add(enemy[i].GetComponent<Transform>());
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
        //물리력을 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //장애물의 위치와 초기 이동 방향 불규칙하게 변경
        tr.localPosition = new Vector3(4.0f, 0.65f, 4.0f);
        targetTr.localPosition = new Vector3(-4.0f, 0.65f, -4.0f);
        targetTr.localRotation = Quaternion.identity;

        setBallsPos();

        StartCoroutine(RevertMaterial());
    }

    public void setBallsPos()
    {
        List<Vector3> randPos = new List<Vector3>();
        for (int fixed_vector = 0; fixed_vector < num_ball;)
        {
            bool add_vector = true;
            Vector3 temp = new Vector3(Random.Range(-2, 2), 0.5f, Random.Range(-2, 2));
            for(int i = 0; i < fixed_vector; i++)
            {
                if (randPos[i] == temp) add_vector = false;
            }
            if (add_vector)
            {
                randPos.Add(temp);
                fixed_vector++;
            }
        }

        for (int i = 0; i < num_ball; i++)
        {
            enemyTr[i].localPosition = randPos[i];
            enemyTr[i].localRotation = Quaternion.identity;
            float randThetha = Random.Range(0, 361) * Mathf.PI / 180;
            enemy[i].GetComponent<BallMoving>().setVelocity(new Vector3(Mathf.Sin(randThetha), 0, Mathf.Cos(randThetha)));
        }
    }

    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor) //8 + 5 * num_ball
    {
        sensor.AddObservation(targetTr.localPosition);  //3 (x,y,z)
        for (int i = 0; i < num_ball; i++)              //3 (x,y,z) & 2(x,z) * num of balls
        {
            sensor.AddObservation(enemy[i].GetComponent<Transform>().localPosition);
            sensor.AddObservation(enemy[i].GetComponent<Rigidbody>().velocity.x);
            sensor.AddObservation(enemy[i].GetComponent<Rigidbody>().velocity.y); //if ball will moving
        }
        sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
        sensor.AddObservation(rb.velocity.x);           //1 (x)
        sensor.AddObservation(rb.velocity.z);           //1 (z)


    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionX = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        var actionZ = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        Vector3 dir = (Vector3.forward * actionZ) + (Vector3.right * actionX);
        rb.velocity = (dir.normalized * speed);

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
            if(distence > 0.5f) SetReward(distance_reward / (2 * distence));
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
