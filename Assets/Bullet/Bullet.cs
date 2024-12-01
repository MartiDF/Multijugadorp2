using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float time;
    float frames = 0;

    [NonSerialized] public Vector3 direccio;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direccio * speed * Time.deltaTime);
        frames += Time.deltaTime;
        if(frames >= time)
        {
            Destroy(this.gameObject);
        }
    }
}
