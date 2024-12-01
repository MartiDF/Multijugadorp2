using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class NonAdaptiveFollow : MonoBehaviour
{
    public Transform player1; // Transform del jugador 1
    public Transform player2; // Transform del jugador 2
    public Transform initialPosition1; // Posici� inicial del jugador 1
    public Transform initialPosition2; // Posici� inicial del jugador 2

    private float stopThreshold = 10f; // Dist�ncia m�xima permesa entre jugadors
    private float smoothSpeed = 3f; // Velocitat d'interpolaci� de la c�mera

    Camera cam;

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
        // Cancel�lar la subscripci� a l'esdeveniment
        MultiplayerManager.OnPlayersSpawned -= AssignPlayers;
    }

    private void AssignPlayers(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;

        Debug.Log($"NonAdaptiveFollow: Jugadors assignats - Player 1: {p1.name}, Player 2: {p2.name}");
    }

    void RestringirMoviment(Transform jugador)
    {
        // Calculem on es troba el l�mit esquerre i dret de la c�mera
        Vector3 plaEsquerre = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.transform.position.z));
        Vector3 plaDret = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.transform.position.z));

        // Calculem la dist�ncia del jugador a cada pla
        float distanciaEsq = Mathf.Abs(jugador.position.x - plaEsquerre.x);
        float distanciaDre = Mathf.Abs(jugador.position.x - plaDret.x);

        float tamanyMig = 0.5f; // Tamany del jugador

        if (distanciaEsq <= tamanyMig)
        {
            jugador.position = new Vector3(plaEsquerre.x + tamanyMig, jugador.position.y, jugador.position.z);
        }
        else if (distanciaDre <= tamanyMig)
        {
            jugador.position = new Vector3(plaDret.x - tamanyMig, jugador.position.y, jugador.position.z);
        }
    }

    void LateUpdate()
    { 
        if (player1 != null && player2 != null)
        {
            Vector3 targetPosition;

            // Posici� mitja entre els jugadors
            Vector2 pos = (player1.position + player2.position) / 2;

            // Calculem la distancia entre els jugadors
            float playersDistance = Mathf.Abs(player1.position.x - player2.position.x);

            if (playersDistance >= stopThreshold)
            {
                RestringirMoviment(player1);
                RestringirMoviment(player2);

                // Fem que la c�mera es pugui moure en l'eix de la Y
                targetPosition = new Vector3(transform.position.x, pos.y, transform.position.z);
            }
            else
            {
                // Fem que la c�mera es col�loqui en un punt mig entre els jugadors
                targetPosition = new Vector3(pos.x, pos.y, transform.position.z);
            }
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        }
        else {
            // Si no hi ha jugadors ni posicions inicials configurades, no facis res
            Debug.LogWarning("Ni jugadors ni posicions inicials disponibles per seguir!");
            return;
        }
    }
}