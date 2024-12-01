using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // Referència al Transform del jugador
    public Transform initialPosition; // Posició inicial on centrar la càmera abans que el jugador es creï
    public Vector3 offset = new Vector3(0, 0, -10); // Offset per mantenir la càmera darrere
    public float smoothSpeed = 0.125f; // Velocitat de transició suau

    private void OnEnable()
    {
        // Subscriure's a l'esdeveniment
        MultiplayerManager.OnPlayersSpawned += AssignPlayerToCamera;
    }

    private void OnDisable()
    {
        // Cancel·lar la subscripció a l'esdeveniment
        MultiplayerManager.OnPlayersSpawned -= AssignPlayerToCamera;
    }

    private void AssignPlayerToCamera(Transform p1, Transform p2)
    {
        // Determina quin jugador ha de seguir aquesta càmera
        if (gameObject.name == "SplitCamera1")
        {
            player = p1;
            Debug.Log($"{gameObject.name} assignada a seguir {p1.name}");
        }
        else if (gameObject.name == "SplitCamera2")
        {
            player = p2;
            Debug.Log($"{gameObject.name} assignada a seguir {p2.name}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} no és SplitCamera1 ni SplitCamera2.");
        }
    }

    private void LateUpdate()
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