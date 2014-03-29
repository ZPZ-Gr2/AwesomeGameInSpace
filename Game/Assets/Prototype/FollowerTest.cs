﻿using UnityEngine;
using System.Collections;

public class FollowerTest : MonoBehaviour {
    NavMeshAgent agent;
    GameObject player;
    AudioSource audio;

    float time;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>().gameObject;
        audio = GetComponent<AudioSource>();
        time = Time.fixedTime + Random.Range(1.0f, 5.0f);
        audio.pitch = Random.Range(0.9f, 1.1f);
	}

    void FixedUpdate()
    {
        agent.SetDestination(player.transform.position);
        if (Time.fixedTime > time)
        {
            audio.Play();
            time += Random.Range(10.0f, 20.0f);
        }
    }
}