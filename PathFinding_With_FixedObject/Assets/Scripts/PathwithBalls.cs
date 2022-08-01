using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

public class PathwithBalls : Agent
{
    public float speed = 5.0f;
    private Transform tr;
    private Rigidbody rb;
    public Transform targetTr;
    public List<GameObject> enemy = new List<GameObject>();
    public int num_ball = 0;

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

        //에이젼트의 위치를 불규칙하게 변경
        tr.localPosition = new Vector3(4.0f, 0.65f, 4.0f);
        targetTr.localPosition = new Vector3(-4.0f, 0.65f, -4.0f);
        targetTr.rotation = Quaternion.identity;

        for(int i = 0; i < num_ball; i++)
        {
            enemy[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            if(Random.value > 0.5f) enemy[i].GetComponent<Transform>().localPosition = new Vector3(Random.Range(-4, 4), 0.65f, Random.Range(-2, 2));
            else enemy[i].GetComponent<Transform>().localPosition = new Vector3(Random.Range(-2, 2), 0.65f, Random.Range(-4, 4));
            enemy[i].GetComponent<Transform>().rotation = Quaternion.identity;
            float randThetha = Random.Range(0, 361) * Mathf.PI / 180;

            //enemy[i].GetComponent<BallMoving>().setVelocity(new Vector3(Mathf.Sin(randThetha), 0, Mathf.Cos(randThetha)));
        }

        StartCoroutine(RevertMaterial());
    }

    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition);  //3 (x,y,z)
        sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
        for (int i = 0; i < num_ball; i++)              //3 (x,y,z) & 2(x,z) * num of balls
        {
            sensor.AddObservation(enemy[i].GetComponent<Transform>().localPosition);
            //sensor.AddObservation(enemy[i].GetComponent<Rigidbody>().velocity.x);
            //sensor.AddObservation(enemy[i].GetComponent<Rigidbody>().velocity.y); //if ball will moving
        }
            
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
            float distence = Vector3.Magnitude(targetTr.localPosition - tr.localPosition);
            if (distence > 0.5f) SetReward(distance_reward / (2 * distence));
            else SetReward(distance_reward);

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
