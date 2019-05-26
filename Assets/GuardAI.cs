using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GuardAI : MonoBehaviour
{
    public float speed = 5f;
    public float waitTime = 0.3f;
    public float turnSpeed = 90f;

    //AI vision
    public float viewDistance;
    public float viewAngle;
    float originalviewAngle; // to store original viewpoint value, to identify changes
    public LayerMask viewMask;

    public Transform pathHolder;
    GameObject player;

    bool firstTimeStartComplete;
    Animator anim;

    //test 
    public GameObject exclamationMark;
    public GameObject questionMark;

    //lookaround
    public float lookAroundTime = 4f;
    float originalLookAroundTime; // ya know 
    bool isMoving;

    //AI ATTACK
    bool isAttacking;
    public int damage;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        originalviewAngle = viewAngle;
        originalLookAroundTime = lookAroundTime;

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);   
        }
        StartCoroutine(FollowPath(waypoints));
        firstTimeStartComplete = true;
    }


    bool CanSeePlayer()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position + new Vector3(0, 1.5f, 0), player.transform.position, viewMask) && player.GetComponent<PlayerStealth>().isHidden != true)
                {
                    return true;
                    }
                if (!Physics.Linecast(transform.position, player.transform.position, viewMask) && player.GetComponent<PlayerStealth>().isHidden != true)
                {
                    return true;
                }
            }
            if (Vector3.Distance(transform.position, player.transform.position) < 2.5f) return true;
        }
        return false;
    }

    void SetGuardState(int state) // 0 = idle 1 = moving 2 = look around   3 = attack
    {
        if (state == 0)
        {
            viewAngle = originalviewAngle; // reset values
            anim.SetBool("isMoving", false);
            anim.SetBool("isAttacking", false);
        }
        else if (state == 1)
        {
            viewAngle = originalviewAngle; // reset values
            anim.SetBool("isMoving", true);
            anim.SetBool("isAttacking", false);
        }
        else if (state == 2)
        {
            anim.SetTrigger("lookAround");
            anim.SetBool("isMoving", false);
            anim.SetBool("isAttacking", false);
        }
        else if (state == 3)
        {
            anim.SetBool("isAttacking", true);
            anim.SetBool("isMoving", false);
        }
    }

    public void AttackPlayer() //need timer (go down to ontriggerstay) !!!!!!!!!!!!!!
    {
        Debug.Log("got attacked by bligger" + " " + damage);
        player.GetComponent<PlayerHealth>().TakeDamage(damage);
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        if (firstTimeStartComplete == false)
        {
            transform.position = waypoints[0];
        }
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);
        while (true)
        {
            anim.SetBool("isMoving", true);
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                SetGuardState(0);
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            if (CanSeePlayer())
            {
                StartCoroutine(GoToPlayer()); 
                break;
            }
            yield return null;
        }
    }

    IEnumerator GoToPlayer()
    {
        //StartCoroutine(TurnToFace(player.position));
        Vector3 pPosition = player.transform.position;
        while (transform.position != player.transform.position)
        {
                if (!CanSeePlayer())
                {
                    Vector3 pLastKnownLocation = player.transform.position;
                    while (Vector3.Distance(transform.position, pLastKnownLocation) > 0.1f)
                    {
                        if (CanSeePlayer())
                        {
                            goto done;
                        }
                        exclamationMark.SetActive(false);
                        questionMark.SetActive(true);
                        transform.position = Vector3.MoveTowards(transform.position, pLastKnownLocation, speed * Time.deltaTime);
                        yield return null;
                    }
                    Vector3[] waypoints = new Vector3[pathHolder.childCount];
                    for (int i = 0; i < waypoints.Length; i++)
                    {
                        waypoints[i] = pathHolder.GetChild(i).position;
                        waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
                    }

                    // replacing waitforseconds
                    while (lookAroundTime > 0)
                    {
                        lookAroundTime -= Time.deltaTime;
                        viewAngle = 180f;
                        SetGuardState(2);
                        if (CanSeePlayer()) goto done;
                        yield return null;
                    }
                    lookAroundTime = originalLookAroundTime;
                    SetGuardState(1);
                    StartCoroutine(FollowPath(waypoints));
                    questionMark.SetActive(false);
                    exclamationMark.SetActive(false);
                    break;
                }
            done:
                lookAroundTime = originalLookAroundTime;
                exclamationMark.SetActive(true);
                questionMark.SetActive(false);
            if (!isAttacking)
            {
                SetGuardState(1);
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * 1.5f * Time.deltaTime);
                yield return StartCoroutine(TurnToFace(player.transform.position));
                yield return null;
            }
            else
            {
                SetGuardState(3);
                yield return null;
            }
            //Debug.Log("xddd");
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) 
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                isAttacking = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            SetGuardState(1);
        }
    }
    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach(Transform waypoint in pathHolder)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(waypoint.position, 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + new Vector3(0, 1.5f, 0), transform.forward * viewDistance);
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
