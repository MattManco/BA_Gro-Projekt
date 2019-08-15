﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineTrigger : MonoBehaviour
{
    public int index;
    public int altIndex;

    public float delay;

    public bool decideBetweenVoicelines = false;

    public bool isDillenVoiceLine = false;

    private void Start()
    {
        if(decideBetweenVoicelines)
        {
            if(Random.Range(0,2) >= 1)
            {
                index = altIndex;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (!isDillenVoiceLine)
                VoiceLines.instance.PlayVoiceLine(index, delay);
            else
                VoiceLines.instance.PlayDillenVoiceLine(index, delay);
            Destroy(gameObject);
        }
    }
}
