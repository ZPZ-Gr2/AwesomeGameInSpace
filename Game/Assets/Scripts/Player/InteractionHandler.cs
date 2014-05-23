﻿using UnityEngine;
using System.Collections;

public class InteractionHandler : MonoBehaviour
{
    public float touchRange = 0.7f;
    public float touchRadius = 0.2f;
    private string message;
    Vector2 screenPosition;

    void FixedUpdate()
    {
        message = null;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, touchRadius, transform.forward, touchRange);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Interactive interactiveObject = hit.collider.GetComponent<Interactive>();
                if (interactiveObject != null)
                {
                    message = interactiveObject.message;
                    screenPosition = Camera.main.WorldToScreenPoint(interactiveObject.transform.position);

                    if (Input.GetKeyDown(KeyCode.E) || Gamepad.instance.justPressedX())
                    {
                        interactiveObject.Action();
                    }

                    break;
                }
            }
        }
    }

    void OnGUI()
    {
        if (!string.IsNullOrEmpty(message))
            GUI.Label(new Rect(screenPosition.x, Screen.height - screenPosition.y, 100, 100), message);
    }
}
