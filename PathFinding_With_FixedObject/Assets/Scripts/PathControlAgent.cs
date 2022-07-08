using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;
using System.Collections;

public class PathControlAgent : Agent
{
    private Transform tr;
    private Rigidbody rb;
    public Transform targetTr;
    public Transform enemyTr;
    public Rigidbody enemyEnemyTr;

    public Renderer floorRd;

    private Material originMt;
    public Material badMt;
    public Material goodMt;

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
        enemyEnemyTr.velocity = Vector3.zero;
        enemyEnemyTr.angularVelocity = Vector3.zero;

        
        tr.localPosition = new Vector3(4.0f, 0.65f, 4.0f);
        targetTr.localPosition = new Vector3(-4.0f, 0.65f,-4.0f);
        //장애물의 위치를 불규칙하게 변경
        enemyTr.localPosition = new Vector3(Random.Range(-3.0f, 3.0f), 0.55f, Random.Range(-3.0f, 3.0f));
        targetTr.localRotation = Quaternion.identity;
        enemyTr.localRotation = Quaternion.identity;

        StartCoroutine(RevertMaterial());
    }

    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition);  //3 (x,y,z)
        sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
        sensor.AddObservation(enemyTr.localPosition);   //3 (x,y,z)
        //sensor.AddObservation(enemyEnemyTr.velocity.x); //1 장애물의 속도 알려주기 위함
        //sensor.AddObservation(enemyEnemyTr.velocity.y); //1 if ball will moving
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
        SetReward(-0.1f);
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
            SetReward(-5.0f);
            //학습을 종료시키는 메소드 - 주석 시 공에 충돌해도 에피소드가 진행됨
            EndEpisode();
        }

        if (coll.collider.CompareTag("wall"))
        {
            SetReward(-0.5f);
        }

        if (coll.collider.CompareTag("target"))
        {
            floorRd.material = goodMt;
            //올바른 행동일 때 플러스 보상을 준다.
            SetReward(+10.0f);
            //학습을 종료시키는 메소드
            EndEpisode();
        }
    }

    /*void OnCollisionStay(Collision coll) //붙어있는 동안에도 벌점 추가. 주석으로 되어 있으면 충돌 시 벌점 1번만 추가
    {
        if (coll.collider.CompareTag("wall"))
        {
            SetReward(-0.5f);
        }

        if (coll.collider.CompareTag("ball"))
        {
            SetReward(-5.0f);
        }
    }*/
}
