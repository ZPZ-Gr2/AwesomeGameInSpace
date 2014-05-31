﻿using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoSingleton<PlayerController>
{
    public float acceleration = 10.0f;
    public float walkSpeed = 4.0f;
    public float sprintSpeed = 6.0f;
    public float jumpHeight = 0.5f;
    public float standingViewHeight = 0.75f;
    public float crouchingViewHeight = -0.25f;
    public float crouchingDelay = 0.4f;
    public float stepLength = 0.94f;
    public List<AudioClip> walkOnMetalSounds;
    public List<AudioClip> dropSounds;
    public List<AudioClip> jumpSounds;
    public List<AudioClip> breathSounds;
    private FirstPersonCameraController firstPersonCameraController;
    private bool isTouchingGround = false;

    public bool isCrouching
    {
        get;
        private set;
    }

    public float speed
    {
        get
        {
            return rigidbody.velocity.magnitude;
        }
    }

    private float crouchingTimeLeft = 0;
    private float distance = 0.0f;
    private bool canJump;
    private CapsuleCollider playerCollider;
    private Vector3 input;

    void Start()
    {
        firstPersonCameraController = transform.GetComponentInChildren<FirstPersonCameraController>();
        playerCollider = GetComponent<CapsuleCollider>();
        input = new Vector3();
    }

    void Update()
    {
        //You can't touch this
        //You can't touch this
        //You can't touch this
        //You can't touch this
        //You can't touch this

        //My, my, my music hits me so hard
        //Makes me say "Oh, my Lord"
        //Thank you for blessing me
        //With a mind to rhyme and two hype feet

        //It feels good, when you know you're down
        //A super dope homeboy from the Oak town
        //And I'm known as such
        //And this is a beat, uh, you can't touch
                
        //I told you, homeboy
        //(You can't touch this)
        //Yeah, that's how we living and you know
        //(You can't touch this)

        //Look at my eyes, man
        //(You can't touch this)
        //Yo, let me bust the funky lyrics
        //(You can't touch this)

        if (!isCrouching && (Input.GetKeyDown(KeyCode.LeftControl) || Gamepad.instance.justPressedY()))
        {
            isCrouching = !isCrouching;
            crouchingTimeLeft = crouchingDelay - crouchingTimeLeft;
        }
        else if (isCrouching && (Input.GetKeyDown(KeyCode.LeftControl) || Gamepad.instance.justPressedY()) && !Physics.Raycast(transform.position, Vector3.up, 1.0f))
        {
            isCrouching = !isCrouching;
            crouchingTimeLeft = crouchingDelay - crouchingTimeLeft;
        }
        canJump |= isTouchingGround && (Time.fixedTime > nextJump) && ((Input.GetAxis("Jump") > 0.5f) || Gamepad.instance.pressedA());
    }

    void FixedUpdate()
    {
        Crouch();
        Walk();
        Jump();
        Breathe();
        isTouchingGround = false;
    }

    void Walk()
    {
        // apply mouse movement
        transform.localRotation = Quaternion.Euler(new Vector3(0, firstPersonCameraController.horizontalAngle, 0));

        // read input
        input.x = Input.GetAxis("Horizontal") + Gamepad.instance.leftStick().x;
        input.y = Input.GetAxis("Vertical") + Gamepad.instance.leftStick().y;

        // normalize if needed to avoid going faster diagonally
        if (input.sqrMagnitude > 1.0f)
            input.Normalize();

        // check if sprinting
        if (isCrouching)
        {
            input *= 0.5f;
        }
        else if ((input.magnitude > 0.0f) && ((Input.GetAxis("Sprint") > 0.5f) || Gamepad.instance.pressedLeftStick()))
        {
            input.y = sprintSpeed / walkSpeed;
        }

        // calculate desired velocity
        input = transform.forward * input.y + transform.right * input.x;

        // calculate difference between desired velocity and current velocity
        Vector3 deltaVelocity = input * walkSpeed - rigidbody.velocity;
        Vector3 relativeDeltaVelocity = deltaVelocity / walkSpeed;
        // ignore vertical component (e.i. falling and jumping
        relativeDeltaVelocity.y = 0;

        // calculate final acceleration
        Vector3 acc = relativeDeltaVelocity.normalized * Mathf.Pow(relativeDeltaVelocity.magnitude, 0.1f) * acceleration;

        float maxAcceleration = deltaVelocity.magnitude / Time.fixedDeltaTime;

        if (acc.magnitude > maxAcceleration)
        {
            acc = acc.normalized * maxAcceleration;
        }

        // apply acceleration
        rigidbody.AddForce(acc, ForceMode.Acceleration);

        if (isTouchingGround)
        {
            distance += Mathf.Sqrt(rigidbody.velocity.magnitude) * Time.fixedDeltaTime;
            if (distance > stepLength)
            {
                distance = 0.0f;
                PlayRandomAudio(walkOnMetalSounds);
            }
        }
        else
        {
            distance = 0.0f;
        }
    }

    private float nextJump = -10000.0f;

    void Jump()
    {
        if (canJump)
        {
            nextJump = Time.fixedTime + 0.5f;
            canJump = false;
            rigidbody.AddForce(Vector3.up * Mathf.Sqrt(2.0f * 9.81f * jumpHeight), ForceMode.VelocityChange);
            PlayRandomAudio(jumpSounds);
        }
    }

    void Crouch()
    {
        if (crouchingTimeLeft < 0.0f)
            crouchingTimeLeft = 0.0f;

        float T = crouchingTimeLeft / crouchingDelay;
        if (!isCrouching)
            T = 1 - T;

        float height = MathfX.sinerp(crouchingViewHeight, standingViewHeight, T);
        firstPersonCameraController.transform.localPosition = new Vector3(0, height, 0);
        height = MathfX.sinerp(-0.5f, 0, T);
        playerCollider.center = new Vector3(0, height, 0);
        height = MathfX.sinerp(1, 2, T);
        playerCollider.height = height;

        crouchingTimeLeft -= Time.fixedDeltaTime;
    }

    private float fatigue = 0.0f;
    private float nextBreath;

    void Breathe()
    {
        // TODO make it better
        float targetFatigue = rigidbody.velocity.magnitude / sprintSpeed;
        fatigue = (fatigue - targetFatigue) * Mathf.Exp(-0.15f * Time.fixedDeltaTime) + targetFatigue;

        if (Time.time > nextBreath)
        {
            nextBreath = Time.time;
            int index = Mathf.FloorToInt(fatigue * breathSounds.Count);

            if (index >= breathSounds.Count)
                index = breathSounds.Count - 1;
            else if (index < 0)
                index = 0;

            AudioClip sound = breathSounds [index];
            audio.PlayOneShot(sound, 0.5f);
            nextBreath = Time.time + sound.length * 0.9f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        isTouchingGround = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.relativeVelocity.y > 4f)
        {
            PlayRandomAudio(dropSounds);
            PlayRandomAudio(walkOnMetalSounds);
        }
    }

    void OnTriggerStay(Collider other)
    {
        isTouchingGround = true;
    }

    void PlayRandomAudio(List<AudioClip> list)
    {
        int count = list.Count;
        if (count != 0)
            audio.PlayOneShot(list [Random.Range(0, count)]);
    }
}
