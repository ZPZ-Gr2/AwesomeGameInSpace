﻿using UnityEngine;
using System.Collections;

public class FirstPersonCameraController : MonoSingleton<FirstPersonCameraController>
{
    private float _horizontalAngle;

    public float horizontalAngle
    {
        get
        {
            return _horizontalAngle;
        }
        set
        {
            _horizontalAngle = Mathf.Repeat(value, 360);
        }
    }

    private float _verticalAngle;

    public float verticalAngle
    {
        get
        {
            return _verticalAngle;
        }
        set
        {
            _verticalAngle = Mathf.Clamp(value, -90, 90);
            transform.localRotation = Quaternion.Euler(new Vector3(-_verticalAngle, 0, 0));
        }
    }

    private float acceleration = 1.0f;
    private float mouseSensitivity = 1.0f;
    private float gamepadSensitivity = 3.0f;
    private float effectiveGamepadSensitivity;
    private bool zoom = false;

    private float pacceleration;
    private float pwalkSpeed;
    private float psprintSpeed;
    private float pjumpHeight;

    private float akrecoil;
    private float akhandling;

    private Vector3 riflePosition;
    private Quaternion rifleRotation;
    private Vector3 rifleZoomPosition;
    private Quaternion rifleZoomRotation;

    void Start()
    {
        pacceleration = PlayerController.instance.acceleration;
        pwalkSpeed = PlayerController.instance.walkSpeed;
        psprintSpeed = PlayerController.instance.sprintSpeed;
        pjumpHeight = PlayerController.instance.jumpHeight;

        akrecoil = LaserRifle.instance.recoil;
        akhandling = LaserRifle.instance.handling;

        riflePosition = LaserRifle.instance.gameObject.transform.localPosition;
        rifleRotation = LaserRifle.instance.gameObject.transform.localRotation;
        rifleZoomPosition = new Vector3(0.0195f, -0.0775f, 0.0f);
        rifleZoomRotation = Quaternion.identity;

		camera.depthTextureMode = DepthTextureMode.Depth;
    }

	void Update() {
		if (Input.GetMouseButtonDown(1)) {
			zoom = !zoom;
		}
		if (Gamepad.instance.isConnected()) {
			if (Gamepad.instance.leftTrigger() > 0.75f) {
				zoom = true;
			}
			else {
				zoom = false;
			}
		}

		if (zoom) {
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 25.0f, Time.deltaTime * 4.0f);
            LaserRifle.instance.gameObject.transform.localPosition = Vector3.Lerp(LaserRifle.instance.gameObject.transform.localPosition, rifleZoomPosition, Time.deltaTime * 4.0f);
            LaserRifle.instance.gameObject.transform.localRotation = Quaternion.Lerp(LaserRifle.instance.gameObject.transform.localRotation, rifleZoomRotation, Time.deltaTime * 4.0f);

			PlayerController.instance.acceleration = pacceleration * 0.5f;
			PlayerController.instance.walkSpeed = pwalkSpeed * 0.5f;
			PlayerController.instance.sprintSpeed = pwalkSpeed * 0.5f;
			PlayerController.instance.jumpHeight = 0.0f;

            LaserRifle.instance.recoil = akrecoil * 0.75f;
            LaserRifle.instance.handling = akhandling * 1.25f;
		}
		else {
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 55.0f, Time.deltaTime * 4.0f);
            LaserRifle.instance.gameObject.transform.localPosition = Vector3.Lerp(LaserRifle.instance.gameObject.transform.localPosition, riflePosition, Time.deltaTime * 4.0f);
            LaserRifle.instance.gameObject.transform.localRotation = Quaternion.Lerp(LaserRifle.instance.gameObject.transform.localRotation, rifleRotation, Time.deltaTime * 4.0f);

			PlayerController.instance.acceleration = pacceleration;
			PlayerController.instance.walkSpeed = pwalkSpeed;
			PlayerController.instance.sprintSpeed = psprintSpeed;
			PlayerController.instance.jumpHeight = pjumpHeight;

            LaserRifle.instance.recoil = akrecoil;
            LaserRifle.instance.handling = akhandling;
		}
	}
    
    void FixedUpdate()
    {
#if UNITY_EDITOR
        if (!Input.GetKey(KeyCode.Escape))
        {
#endif
            // lock and hide cursor
            Screen.lockCursor = true;

            if(Gamepad.instance.isConnected())
            {
                RaycastHit hitInfo;
                Vector3 start = Camera.main.transform.position;
                Vector3 dir = Camera.main.transform.forward;

                bool hit = Physics.SphereCast(start, 0.1f, dir, out hitInfo, 20.0f, Layers.enemy);

                if (hit)
                {
                    Alien ai = (Alien)hitInfo.collider.transform.root.GetComponent<Alien>();
                    
                    if (ai != null)
                    {
                        if (!ai.isDead) effectiveGamepadSensitivity = gamepadSensitivity * 0.1f;
                    }
                }
                else
                {
                    effectiveGamepadSensitivity = gamepadSensitivity;
                }
            }

            // read input
            Vector2 input;
            input.x = Input.GetAxisRaw("Mouse X") * mouseSensitivity + Gamepad.instance.rightStick().x * effectiveGamepadSensitivity;
            input.y = Input.GetAxisRaw("Mouse Y") * mouseSensitivity + Gamepad.instance.rightStick().y * effectiveGamepadSensitivity;
            //input /= Screen.height;

            if (input != Vector2.zero)
            {
                float speed = input.magnitude / Time.fixedDeltaTime;
                float multiplier = Mathf.Pow(speed, acceleration);

                input = input.normalized * (multiplier * Time.fixedDeltaTime);

                horizontalAngle += input.x;
                verticalAngle += input.y;
            }
            
#if UNITY_EDITOR
        }
        else
        {
            Screen.lockCursor = false;
        }
#endif
    }
}
