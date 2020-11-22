using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float distanceAway = 5;
    [SerializeField] private float distanceUp = 2;
    [SerializeField] private float smooth = 3;
    [SerializeField] private Transform follow;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        follow = GameObject.FindWithTag("Player").transform;
    }

    void LateUpdate()
    {
        //setting the offset
        targetPosition = follow.position + Vector3.up * distanceUp - follow.forward * distanceAway;

        //making the transition from it's position to the new position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);

        //camera looking the right way
        transform.LookAt(follow);
    }
}
