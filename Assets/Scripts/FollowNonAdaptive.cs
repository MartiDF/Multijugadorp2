using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class NonAdaptiveFollow : MonoBehaviour
{
    public Transform player1; // Transform del jugador 1
    public Transform player2; // Transform del jugador 2
    public Transform initialPosition1; // Posici� inicial del jugador 1
    public Transform initialPosition2; // Posici� inicial del jugador 2

    private bool isCameraLocked = false; // Estat de bloqueig de la c�mera
    private float stopThreshold = 10f; // Dist�ncia m�xima permesa entre jugadors
    private float smoothSpeed = 0.25f; // Velocitat d'interpolaci� de la c�mera
    private float lockSmoothSpeed = 0.0001f; // Velocitat de transici� entre estats
    private float cameraLimit; // L�mits din�mics per al jugador m�s avan�at
    private float maxAllowedPosition; // L�mits din�mics per al jugador m�s avan�at
    private float minAllowedPosition; // L�mits din�mics per al jugador menys avan�at
    private Vector3 initialPosition; // Posici� inicial comuna per calcular qui est� m�s avan�at
    private Vector3 initialCameraTarget; // Objectiu inicial de la c�mera

    
    


    private float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect; //Amplada de la c�mera

    void Start()
    {
        Debug.Log("NonAdaptiveFollow: MultiplayerManager.Instance �s null? " + (MultiplayerManager.Instance == null));
        // Calcular el punt inicial de la c�mera entre les posicions inicials
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
        // Cancel�lar la subscripci� a l'esdeveniment
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

    // Funci� per comprovar si el jugador l�der es mou
    private bool leadingPlayerNotMoving(Transform leadingPlayer)
    {
        if (leadingPlayer == null)
            return false;

        Player p = leadingPlayer.gameObject.GetComponent<Player>();

        // Comprova si la posici� del jugador l�der ha canviat m�s del threshold
        bool notMoving = p.moveValue == Vector2.zero;

        return notMoving;
    }

    void LateUpdate()
    {
        // Si no hi ha refer�ncies als jugadors, busca-les a la jerarquia
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
            // C�lcul normal quan els jugadors estan instanciats
            float distancePlayer1 = Mathf.Abs(player1.position.x - initialCameraTarget.x);
            float distancePlayer2 = Mathf.Abs(player2.position.x - initialCameraTarget.x);


            // Determinar quin jugador est� m�s lluny de la posici� inicial
            Transform leadingPlayer = (distancePlayer1 > distancePlayer2) ? player1 : player2;
            Transform trailingPlayer = (leadingPlayer == player1) ? player2 : player1;

            // Calcular la posici� de bloqueig de la c�mera
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
            
            // Posici� objectiu de la c�mera (seguint el jugador m�s lluny de la posici� inicial)
            // Descongelar la c�mera si els jugadors es tornen a apropar
            float playersDistance = Mathf.Abs(player1.position.x - player2.position.x);
            if (playersDistance < stopThreshold)
            {
                // Desbloquejar c�mera si els jugadors es troben dins del l�mit
                isCameraLocked = false;

                targetPosition = new Vector3(
                    leadingPlayer.position.x, // La c�mera segueix el jugador l�der,
                    transform.position.y,
                    transform.position.z
                );
            }
            else // Congelar la c�mera i ajustar el l�mit m�xim de moviment
            {
                // Bloquejar c�mera si el jugador l�der supera el l�mit
                isCameraLocked = true;

                // La c�mera es mant� fixa
                targetPosition = new Vector3(
                    cameraLimit,
                    transform.position.y,
                    transform.position.z
                );

                // Calcular la posici� de m�xima del jugador m�s avan�at, tenint en compte la seva direcci�
                if (IsNegative(leadingPlayer.position.x))
                {
                    maxAllowedPosition = cameraLimit - 8;

                    // Si el l�der intenta superar el l�mit, bloqueja'l
                    if (leadingPlayer.position.x < maxAllowedPosition)
                    {
                        leadingPlayer.position = new Vector3(
                            maxAllowedPosition, // Bloqueja la posici� al l�mit m�xim
                            leadingPlayer.position.y,
                            leadingPlayer.position.z
                        );
                    }
                }
                else
                {
                    maxAllowedPosition = cameraLimit + 8;

                    // Si el l�der intenta superar el l�mit, bloqueja'l
                    if (leadingPlayer.position.x > maxAllowedPosition)
                    {
                        leadingPlayer.position = new Vector3(
                            maxAllowedPosition, // Bloqueja la posici� al l�mit m�xim
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
                    // Bloquejar el moviment del jugador menys avan�at si intenta sortir de la c�mera estant a la dreta
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
                    // Bloquejar el moviment del jugador menys avan�at si intenta sortir de la c�mera estant a l'esquerra
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
                // Transici� suau cap a la posici� bloquejada
                transform.position = Vector3.Lerp(transform.position, targetPosition, lockSmoothSpeed);
            }
            else
            {
                // Moviment suau cap al jugador l�der
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            }
        }
        else if (initialPosition1 != null && initialPosition2 != null)
        {
            // Si els jugadors no estan instanciats, centra la c�mera entre les posicions inicials
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