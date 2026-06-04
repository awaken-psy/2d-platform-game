using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动锯陷阱：在子对象（Trap_SawWayPoint）定义的路径点之间往返移动，到达端点后停顿 cooldown 秒并翻转 Sprite。
/// </summary>
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

    /// <summary>
    /// 从子对象收集路径点并缓存其世界坐标，便于运行时移动计算。
    /// 路径点数量变化时会自动重建数组。
    /// </summary>
    private void UpdateWayPointsInfo() {
        List<Trap_SawWayPoint> wayPointList = new List<Trap_SawWayPoint>(GetComponentsInChildren<Trap_SawWayPoint>());

        if (wayPointList.Count != wayPoint.Length) {
            wayPoint = new Transform[wayPointList.Count];

            for (int i = 0; i < wayPointList.Count; i++) {
                wayPoint[i] = wayPointList[i].transform;
            }
        }

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