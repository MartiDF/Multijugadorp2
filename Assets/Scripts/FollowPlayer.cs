using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // Referència al Transform del jugador
    public Transform initialPosition; // Posició inicial on centrar la càmera abans que el jugador es creï
    public Vector3 offset = new Vector3(0, 0, -10); // Offset per mantenir la càmera darrere
    public float smoothSpeed = 0.125f; // Velocitat de transició suau

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
            // Si el jugador no existeix, centra la càmera a la posició inicial
            targetPosition = initialPosition.position + offset;
        }
        else
        {
            // Si tampoc hi ha posició inicial, no facis res
            return;
        }

        // Moviment suau cap a la posició objectiu
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
