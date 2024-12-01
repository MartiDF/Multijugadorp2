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
    public float minZoom = 5f; // Zoom mínim de la càmera
    public float maxZoom = 20f; // Zoom màxim de la càmera
    public float zoomLimiter = 10f; // Limitador del zoom segons la distància
    public Camera cam;
    private Vector3 initialCameraTarget; // Objectiu inicial de la càmera



    void Start()
    {
        cam = GetComponent<Camera>();

        Debug.Log("NonAdaptiveFollow: MultiplayerManager.Instance és null? " + (MultiplayerManager.Instance == null));
        // Calcular el punt inicial de la càmera entre les posicions inicials
        if (initialPosition1 != null && initialPosition2 != null)
        {
            initialCameraTarget = (initialPosition1.position + initialPosition2.position) / 2;
        }
        else
        {
            Debug.LogError("Les posicions inicials no estan configurades correctament a l'Inspector.");
        }
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
        // Si no hi ha referències als jugadors, busca-les a la jerarquia
        if (player1 == null)
        {
            GameObject foundPlayer1 = GameObject.Find("Player 1 (Clone)");
            if (foundPlayer1 != null)
            {
                player1 = foundPlayer1.transform;
            }

            if (foundPlayer1 != null && player1 == null)
            {
                Debug.Log("Player 1 referenciat correctament.");
            }
        }

        if (player2 == null)
        {
            GameObject foundPlayer2 = GameObject.Find("Player 2 (Clone)");
            if (foundPlayer2 != null)
            {
                player2 = foundPlayer2.transform;
            }
            if (foundPlayer2 != null && player2 == null)
            {
                Debug.Log("Player 2 referenciat correctament.");
            }
        }

        Vector3 targetPosition;

        if (player1 != null && player2 != null)
        {
            // Si els jugadors estan instanciats, centra la càmera entre ells
            Vector3 middlePoint = (player1.position + player2.position) / 2;
            targetPosition = new Vector3(middlePoint.x, middlePoint.y, -10);

            // Ajusta el zoom segons la distància entre els jugadors
            float distance = Vector3.Distance(player1.position, player2.position);
            float zoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);
            cam.orthographicSize = Mathf.Clamp(zoom, minZoom, maxZoom);
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