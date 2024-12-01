using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdaptiveFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public Transform initialPosition1; // Posició inicial del jugador 1
    public Transform initialPosition2; // Posició inicial del jugador 2
    float minZoom = 5f; // Zoom mínim de la càmera
    float maxZoom = 20f; // Zoom màxim de la càmera
    public float zoomLimiter = 1f; // Limitador del zoom segons la distància
    public Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        // Subscriure's a l'esdeveniment
        MultiplayerManager.OnPlayersSpawned += AssignPlayers;
    }

    void OnDisable()
    {
        // Cancel·lar la subscripció a l'esdeveniment
        MultiplayerManager.OnPlayersSpawned -= AssignPlayers;
    }

    private void AssignPlayers(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;

        Debug.Log($"NonAdaptiveFollow: Jugadors assignats - Player 1: {p1.name}, Player 2: {p2.name}");
    }

    void LateUpdate()
    {
        
        Vector3 targetPosition;

        if (player1 != null && player2 != null)
        {
            // Si els jugadors estan instanciats, centra la càmera entre ells
            Vector3 middlePoint = (player1.position + player2.position) / 2;
            targetPosition = new Vector3(middlePoint.x, middlePoint.y, -10);

            // Ajusta el zoom segons la distància entre els jugadors
            float distance = Vector3.Distance(player1.position, player2.position);
            float zoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
            cam.orthographicSize = Mathf.Clamp(zoom, minZoom, maxZoom);
            //cam.orthographicSize = Mathf.Clamp(zoom, minZoom, maxZoom);
        }
        else if (initialPosition1 != null && initialPosition2 != null)
        {
            // Si els jugadors no estan instanciats, centra la càmera entre les posicions inicials
            Vector3 middlePoint = (initialPosition1.position + initialPosition2.position) / 2;
            targetPosition = new Vector3(middlePoint.x, middlePoint.y, -10);

            // Zoom inicial estàndard
            cam.orthographicSize = Mathf.Lerp(maxZoom, minZoom, 0.5f);
            Debug.LogWarning("Pussy");
        }
        else
        {
            // Si no hi ha jugadors ni posicions inicials, no facis res
            Debug.LogWarning("Ni jugadors ni posicions inicials disponibles per seguir!");
            return;
        }

        // Moviment suau cap a la posició objectiu
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.125f);
    }
}