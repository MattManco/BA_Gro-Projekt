﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerAnimationEvents : MonoBehaviour
{
    private RigidbodyFirstPersonController fpController;

    [SerializeField] AudioSource dyingSound;
    [SerializeField] AudioSource collapseSound;

    // Start is called before the first frame update
    void Start()
    {
        fpController = GetComponentInParent<RigidbodyFirstPersonController>();
    }

    private void FreezeMovement()
    {
        fpController.freezePlayerMovement = true;
    }

    private void UnfreezePlayerMovement()
    {
        fpController.freezePlayerMovement = false;
    }

    private void FreezeCamera()
    {
        fpController.freezePlayerCamera = true;
    }

    private void UnfreezCamera()
    {
        fpController.freezePlayerCamera = false;
    }

    private void PlayDyingSound()
    {
        dyingSound.Play();
    }

    private void PlayCollapseSound()
    {
        collapseSound.Play();
    }

}
