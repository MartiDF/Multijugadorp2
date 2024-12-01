using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // Refer�ncia al Transform del jugador
    public Transform initialPosition; // Posici� inicial on centrar la c�mera abans que el jugador es cre�
    public Vector3 offset = new Vector3(0, 0, -10); // Offset per mantenir la c�mera darrere
    public float smoothSpeed = 0.125f; // Velocitat de transici� suau

    void LateUpdate()
    {
        Vector3 targetPosition;

        if (player != null)
        {
            // Si el jugador existeix, segueix-lo
            targetPosition = player.position + offset;
        }
        else if (initialPosition != null)
        {
            // Si el jugador no existeix, centra la c�mera a la posici� inicial
            targetPosition = initialPosition.position + offset;
        }
        else
        {
            // Si tampoc hi ha posici� inicial, no facis res
            return;
        }

        // Moviment suau cap a la posici� objectiu
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
