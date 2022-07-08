using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;

public class PathwithBalls : Agent
{
    private Transform tr;
    private Rigidbody rb;
    public Transform targetTr;
    public List<GameObject> enemy = new List<GameObject>();
    //해당 Prefab에서는 장애물의 Kinematic으로 처리하여 외력(Agent와의 충돌)에 의해 움직이지 않음
    public int num_ball = 0; //장애물의 개수, 유니티 에디터 - Prefab에서 조절 (권장)

    public Renderer floorRd;

    private Material originMt;
    public Material badMt; //장애물과 충돌 시 바닥 색
    public Material goodMt; //타겟과 충돌 시 바닥 색

    public override void Initialize() //학습 시작 시 처음 1번 실행
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

    public override void OnEpisodeBegin() //에피소트 시작 시 1번 씩 실행
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
        }

        StartCoroutine(RevertMaterial());
    }

    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor) //에피소드 진행 중에 계속 실행
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
        rb.velocity = (dir.normalized * 5.0f);

        //지속적으로 이동을 이끌어내기 위한 마이너스 보상
        SetReward(-0.01f);
    }


    public override void Heuristic(in ActionBuffers actionsOut) //유저 input을 위한 디버그용 함수
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    void OnCollisionEnter(Collision coll) //물체(Rigid body)와 충돌 시 1번 실행
    {
        if (coll.collider.CompareTag("ball"))
        {
            floorRd.material = badMt;
            //잘못된 행동일 때 마이너스 보상을 준다.
            
            SetReward(-5.0f);
            StartCoroutine(RevertMaterial());
        }

        if (coll.collider.CompareTag("wall"))
        {
            SetReward(-1.0f);
        }

        if (coll.collider.CompareTag("target"))
        {
            floorRd.material = goodMt;
            //올바른 행동일 때 플러스 보상을 준다.
            SetReward(+50.0f);
            //학습을 종료시키는 메소드
            EndEpisode();
        }
    }

    void OnCollisionStay(Collision coll) //물체(Rigid body)와 충돌 지속 시 계속 실행
    {
        if (coll.collider.CompareTag("wall"))
        {
            SetReward(-1.0f);
        }

        if (coll.collider.CompareTag("ball"))
        {
            SetReward(-5.0f);
        }
    }
}
