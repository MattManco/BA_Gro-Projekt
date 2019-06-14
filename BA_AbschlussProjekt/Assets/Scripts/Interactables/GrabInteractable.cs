﻿using System;
using CustomEnums;
using UnityEngine;
using System.Linq;

/// <summary>
/// Used for objects that can be carried or pushed
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GrabInteractable : BaseInteractable
{
    /// <summary>
    /// Able to carry object if player condition matches any of the selected
    /// </summary>
    [EnumFlag]
    public InteractionConditions possibleConditionsToCarry = InteractionConditions.All;
    
    /// <summary>
    /// Able to carry object if player condition matches any of the selected
    /// </summary>
    [EnumFlag]
    public InteractionConditions possibleConditionsToPush = InteractionConditions.Unable;

    protected Rigidbody rigid;
    protected Rigidbody rigidbodyPulling;

    //Sound
    protected AudioManager audioManager;
    [SerializeField]
    private float velocity;
    [SerializeField]
    protected AudioSource[] sounds;
    [SerializeField]
    protected string[] soundNames;

    public bool IsBeeingCarried { get; protected set; }

    public bool IsBeeingPulled { get; protected set; }

    protected enum SoundTypes
    {
        pickup = 0,
        drop = 1 //TODO: override
    }


    protected void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public override bool Interact(InteractionScript player)
    {
        InteractionConditions playersConditions = player.PlayerHealth.GetInjuriesAsInteractionConditions();

        if (possibleConditionsToCarry.HasFlag(playersConditions))
        {
            if (GetComponent<AudioSource>() != null)
            {
                PlaySound(soundNames[(int)SoundTypes.pickup]);
            }
            return CarryOutInteraction_Carry(player);
        }

        if (possibleConditionsToPush.HasFlag(playersConditions))
        {
            return CarryOutInteraction_Push(player);
        }

        return false;
    }

    protected virtual bool CarryOutInteraction_Carry(InteractionScript player)
    {
        gameObject.layer = LayerMask.NameToLayer("NoPlayerCollision");
        transform.parent = player.GrabingPoint.transform;
        transform.localPosition = Vector3.zero;
        rigid.isKinematic = true;
        player.SetCarriedObject(this);
        IsBeeingCarried = true;

        return true;
    }

    public virtual void PutDown(InteractionScript player)  //TODO: better putDown implementation instead of simply droping the object
    {
        transform.parent = InstancePool.transform;
        rigid.isKinematic = false;
        player.StopUsingObject();

        IsBeeingPulled = false;
        IsBeeingCarried = false;
        rigidbodyPulling = null;

        Invoke("ResetLayer", 2f); //TODO: Switch to better implementation of invoking ResetLayer. (Maybe with trigger or distance check)
    }

    protected virtual bool CarryOutInteraction_Push(InteractionScript player)
    {
        player.SetPushedObject(this);

        IsBeeingPulled = true;
        rigidbodyPulling = player.GetComponent<Rigidbody>();

        return true;
    }

    private void FixedUpdate()
    {
        velocity = rigid.velocity.y;
        //TODO: better implementation of pulling. Maybe considering objects weight and players conditions
        if (IsBeeingPulled)
            rigid.velocity = rigidbodyPulling.velocity;     
    }

    protected void ResetLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");

    }

    /// <summary>
    /// Play a sound of a type eg. pickup, drop
    /// </summary>
    /// <param name="soundType">type eg. pickup, drop</param>
    protected void PlaySound(string soundType)
    {
        if (GetComponent<AudioSource>() != null)
        {
            sounds = GetComponents<AudioSource>();
            foreach (AudioSource sound in sounds)
            {
                if (sound.clip.name == soundType)
                {
                    sound.Play();
                }
                else
                {
                    audioManager.AddSound(soundType, this.gameObject);
                    sounds = GetComponents<AudioSource>();
                    sounds.First(audios => audios.name == soundType).Play();
                }
            }
        }
        else
        {
            audioManager.AddSound(soundType, this.gameObject);
            GetComponent<AudioSource>().Play();
        }
    }

    /// <summary>
    /// play dropsound when falling from high
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if (velocity < -3)
        {
            if(GetComponent<AudioSource>() != null)
            {
                PlaySound(soundNames[(int)SoundTypes.drop]);
            }
        }
    }

}