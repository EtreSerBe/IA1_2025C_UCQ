using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public int totalDowns;
    public int totalDeaths;
}

public class Scoreboard : MonoBehaviour
{

    public Dictionary<Player, PlayerStats> _PlayersStats = new Dictionary<Player, PlayerStats>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var player in players)
        {
            _PlayersStats.Add(player, new PlayerStats());
        }

        Player.playerDownedEvent += OnPlayerDowned;
        Player.playerDiedEvent += OnPlayerDied;
    }

    private void OnDisable()
    {
        // Aquí nos desuscribimos de este evento cuando este script está deshabilitado para no responder
        // a dichos eventos mientras.
        Player.playerDownedEvent -= OnPlayerDowned;
        Player.playerDiedEvent -= OnPlayerDied;
    }

    private void OnPlayerDowned(Player player)
    {
        // Incrementa el número de downeds de ese players en 1.
        _PlayersStats[player].totalDowns++;
        Debug.Log($"El número de downs del player: {player.gameObject.name} es de: " +
                  $"{_PlayersStats[player].totalDowns}");
    }
    
    private void OnPlayerDied(Player player)
    {
        // Incrementa el número de downeds de ese players en 1.
        _PlayersStats[player].totalDeaths++;
        Debug.Log($"El número de death del player: {player.gameObject.name} es de: " +
                  $"{_PlayersStats[player].totalDeaths}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
