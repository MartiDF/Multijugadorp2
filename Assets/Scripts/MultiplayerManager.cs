using System.Collections;
using System.Collections.Generic;
using System; // Necessari per als esdeveniments
using UnityEngine;
using UnityEngine.InputSystem; // Necessari per al Player Input Manager

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public static event Action<Transform, Transform> OnPlayersSpawned;

    private List<Transform> players = new List<Transform>(); // Per guardar referències dels jugadors

    private void Start()
    {
        StartCoroutine(WaitAndFindPlayers());
    }

    private IEnumerator WaitAndFindPlayers()
    {
        while (true)
        {
            FindAndAssignPlayers();

            // Si els jugadors ja han estat trobats, atura la cerca
            if (GameObject.Find("Player 1(Clone)") != null && GameObject.Find("Player 2(Clone)") != null)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.5f); // Torna a intentar després de mig segon
        }
    }

    private void Awake()
    {
        // Assegura que només hi ha una instància
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Manté l'objecte a través d'escenes
        Debug.Log("MultiplayerManager configurat correctament com a Singleton.");
    }

    private void OnEnable()
    {
        // Registra't als esdeveniments del Player Input Manager
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
    }

    private void OnDisable()
    {
        // Cancel·la la subscripció per evitar problemes
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft -= OnPlayerLeft;
    }

    private void FindAndAssignPlayers()
    {
        GameObject foundPlayer1 = GameObject.Find("Player 1(Clone)");
        GameObject foundPlayer2 = GameObject.Find("Player 2(Clone)");

        if (foundPlayer1 != null && foundPlayer2 != null)
        {
            Debug.Log("Jugadors trobats correctament: Player 1(Clone) i Player 2(Clone).");
            OnPlayersSpawned?.Invoke(foundPlayer1.transform, foundPlayer2.transform);
        }
        else
        {
            Debug.LogWarning("No s'han trobat els jugadors correctes a l'escena.");
        }
    }


    private void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log($"Jugador {playerInput.playerIndex + 1} s'ha unit al joc.");
        players.Add(playerInput.transform);

        // Si tenim exactament 2 jugadors, llança l'esdeveniment
        if (players.Count == 2)
        {
            Debug.Log("MultiplayerManager: Llançant esdeveniment OnPlayersSpawned.");
            OnPlayersSpawned?.Invoke(players[0], players[1]);
        }
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log($"Jugador {playerInput.playerIndex + 1} ha sortit del joc.");
        players.Remove(playerInput.transform);
    }
}
