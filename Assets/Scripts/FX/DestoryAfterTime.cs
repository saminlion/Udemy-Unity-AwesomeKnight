﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryAfterTime : MonoBehaviour
{
    public float timer = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);    
    }
}
