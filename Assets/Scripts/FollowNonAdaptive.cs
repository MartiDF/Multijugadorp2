using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class NonAdaptiveFollow : MonoBehaviour
{
    public Transform player1; // Transform del jugador 1
    public Transform player2; // Transform del jugador 2
    public Transform initialPosition1; // Posició inicial del jugador 1
    public Transform initialPosition2; // Posició inicial del jugador 2

    private bool isCameraLocked = false; // Estat de bloqueig de la càmera
    private float stopThreshold = 10f; // Distància màxima permesa entre jugadors
    private float smoothSpeed = 0.25f; // Velocitat d'interpolació de la càmera
    private float lockSmoothSpeed = 0.0001f; // Velocitat de transició entre estats
    private float cameraLimit; // Límits dinàmics per al jugador més avançat
    private float maxAllowedPosition; // Límits dinàmics per al jugador més avançat
    private float minAllowedPosition; // Límits dinàmics per al jugador menys avançat
    private Vector3 initialPosition; // Posició inicial comuna per calcular qui està més avançat
    private Vector3 initialCameraTarget; // Objectiu inicial de la càmera

    
    


    private float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect; //Amplada de la càmera

    void Start()
    {
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

    bool IsNegative(float number)
    {
        return number < 0;
    }

    // Funció per comprovar si el jugador líder es mou
    private bool leadingPlayerNotMoving(Transform leadingPlayer)
    {
        if (leadingPlayer == null)
            return false;

        Player p = leadingPlayer.gameObject.GetComponent<Player>();

        // Comprova si la posició del jugador líder ha canviat més del threshold
        bool notMoving = p.moveValue == Vector2.zero;

        return notMoving;
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
            // Càlcul normal quan els jugadors estan instanciats
            float distancePlayer1 = Mathf.Abs(player1.position.x - initialCameraTarget.x);
            float distancePlayer2 = Mathf.Abs(player2.position.x - initialCameraTarget.x);


            // Determinar quin jugador està més lluny de la posició inicial
            Transform leadingPlayer = (distancePlayer1 > distancePlayer2) ? player1 : player2;
            Transform trailingPlayer = (leadingPlayer == player1) ? player2 : player1;

            // Calcular la posició de bloqueig de la càmera
            if (IsNegative(leadingPlayer.position.x))
            {
                cameraLimit = trailingPlayer.position.x - stopThreshold; // Per l'esquerra
                minAllowedPosition = leadingPlayer.position.x + stopThreshold + 7;
            }
            else
            {
                cameraLimit = trailingPlayer.position.x + stopThreshold; // Per la dreta
                minAllowedPosition = leadingPlayer.position.x - stopThreshold - 7;
            }
            
            // Posició objectiu de la càmera (seguint el jugador més lluny de la posició inicial)
            // Descongelar la càmera si els jugadors es tornen a apropar
            float playersDistance = Mathf.Abs(player1.position.x - player2.position.x);
            if (playersDistance < stopThreshold)
            {
                // Desbloquejar càmera si els jugadors es troben dins del límit
                isCameraLocked = false;

                targetPosition = new Vector3(
                    leadingPlayer.position.x, // La càmera segueix el jugador líder,
                    transform.position.y,
                    transform.position.z
                );
            }
            else // Congelar la càmera i ajustar el límit màxim de moviment
            {
                // Bloquejar càmera si el jugador líder supera el límit
                isCameraLocked = true;

                // La càmera es manté fixa
                targetPosition = new Vector3(
                    cameraLimit,
                    transform.position.y,
                    transform.position.z
                );

                // Calcular la posició de màxima del jugador més avançat, tenint en compte la seva direcció
                if (IsNegative(leadingPlayer.position.x))
                {
                    maxAllowedPosition = cameraLimit - 8;

                    // Si el líder intenta superar el límit, bloqueja'l
                    if (leadingPlayer.position.x < maxAllowedPosition)
                    {
                        leadingPlayer.position = new Vector3(
                            maxAllowedPosition, // Bloqueja la posició al límit màxim
                            leadingPlayer.position.y,
                            leadingPlayer.position.z
                        );
                    }
                }
                else
                {
                    maxAllowedPosition = cameraLimit + 8;

                    // Si el líder intenta superar el límit, bloqueja'l
                    if (leadingPlayer.position.x > maxAllowedPosition)
                    {
                        leadingPlayer.position = new Vector3(
                            maxAllowedPosition, // Bloqueja la posició al límit màxim
                            leadingPlayer.position.y,
                            leadingPlayer.position.z
                        );
                    }
                }
            }

            if (leadingPlayerNotMoving(leadingPlayer))
            {
                if (IsNegative(leadingPlayer.position.x))
                {
                    // Bloquejar el moviment del jugador menys avançat si intenta sortir de la càmera estant a la dreta
                    if (trailingPlayer.position.x > minAllowedPosition)
                    {
                        trailingPlayer.position = new Vector3(
                            minAllowedPosition,
                            trailingPlayer.position.y,
                            trailingPlayer.position.z
                        );
                    }
                }
                else
                {
                    // Bloquejar el moviment del jugador menys avançat si intenta sortir de la càmera estant a l'esquerra
                    if (trailingPlayer.position.x < minAllowedPosition)
                    {
                        trailingPlayer.position = new Vector3(
                            minAllowedPosition,
                            trailingPlayer.position.y,
                            trailingPlayer.position.z
                        );
                    }
                }
            }

            if (isCameraLocked)
            {
                // Transició suau cap a la posició bloquejada
                transform.position = Vector3.Lerp(transform.position, targetPosition, lockSmoothSpeed);
            }
            else
            {
                // Moviment suau cap al jugador líder
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            }
        }
        else if (initialPosition1 != null && initialPosition2 != null)
        {
            // Si els jugadors no estan instanciats, centra la càmera entre les posicions inicials
            targetPosition = new Vector3(
                initialCameraTarget.x,
                transform.position.y,
                transform.position.z
            );           
        }
        else {
            // Si no hi ha jugadors ni posicions inicials configurades, no facis res
            Debug.LogWarning("Ni jugadors ni posicions inicials disponibles per seguir!");
            return;
        }
    }
}