using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    Player player;
    GameManager gameManager;
    
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gameManager.InstanciarJugador();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(player)
            player.Move(context.ReadValue<Vector2>());
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if(player && context.started)
        {
            player.Shoot();
        }
    }
}
