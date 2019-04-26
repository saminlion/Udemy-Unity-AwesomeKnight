using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followHeight = 8.0f;
    public float followDistance = 6.0f;

    private Transform player;

    private float targetHeight;
    private float currentHeight;
    private float currentRotation;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        targetHeight = player.position.y + followHeight;

        currentRotation = transform.eulerAngles.y;

        currentHeight = Mathf.Lerp(transform.position.y, targetHeight, 0.9f * Time.deltaTime);

        Quaternion euler = Quaternion.Euler(0f, currentRotation, 0f);

        Vector3 targetPos = player.position - (euler * Vector3.forward) * followDistance;

        targetPos.y = currentHeight;

        transform.position = targetPos;
        transform.LookAt(player);
    }

} // class
