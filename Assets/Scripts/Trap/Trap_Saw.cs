using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private float movespeed = 3f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private Transform[] wayPoint;

    [SerializeField] private Vector3[] wayPointPosition;

    public int wayPointIndex = 1;
    public int moveDir = 1;
    private bool canMove = true;

    private void Awake() {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        UpdateWayPointsInfo();
        transform.position = wayPointPosition[0];
    }

    private void UpdateWayPointsInfo() {
        // 从子对象中获取所有路径点组件
        List<Trap_SawWayPoint> wayPointList = new List<Trap_SawWayPoint>(GetComponentsInChildren<Trap_SawWayPoint>());

        // 若路径点数量发生变化，重建 Transform 数组
        if (wayPointList.Count != wayPoint.Length) {
            wayPoint = new Transform[wayPointList.Count];

            for (int i = 0; i < wayPointList.Count; i++) {
                wayPoint[i] = wayPointList[i].transform;
            }
        }

        // 固定路径点的世界坐标
        wayPointPosition = new Vector3[wayPoint.Length];

        for (int i = 0; i < wayPoint.Length; i++) {
            wayPointPosition[i] = wayPoint[i].position;
        }
    }

    private void Update() {
        anim.SetBool("active", canMove);
        if (!canMove)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPointPosition[wayPointIndex], movespeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) < 0.1f) {
            if (wayPointIndex == wayPointPosition.Length - 1 || wayPointIndex == 0) {
                StartCoroutine(StopMovement(cooldown));
                moveDir *= -1;
            }
            wayPointIndex += moveDir;
        }
    }

    private IEnumerator StopMovement(float delay) {
        canMove = false;
        yield return new WaitForSeconds(delay);
        canMove = true;
        sr.flipX = !sr.flipX;
    }
}