using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

/// <summary>
/// Una Entidad con capacidades de percibir su ambiente, a sí mismo y a otros agentes y actuar con respecto a lo que percibe.
/// </summary>
public class Agent : MonoBehaviour
{
    private Senses _senses; // La clase que le permite percibir su ambiente y a otros agentes.
    
    // Percibirse a sí mismo es un poco tramposo porque es tener en cuenta la variables que este agente va a tener internamente.
    // Aquí no hay un script en sí, porque sería este mismo de Agent, son las cosas que puede saber de sí mismo.
    // Ejemplos: Si tiene mucha o poca HP,
    // Si todavía tiene balas o ya no.

    private SteeringBehaviors _steeringBehaviors;
    
    // si detectaste a algún gameObject que tenga la tag de player, persíguelo.
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> foundPlayers = _senses.GetPlayers();
        // de los players encontrados, vamos a irnos por el que esté más cerca de este agente.
        // el que esté más cerca será el player que tenga la menor distancia con respecto a este agente.
        float shortestDistance = float.PositiveInfinity;
        GameObject nearestPlayer = null;
        foreach (var player in foundPlayers)
        {
            // obtenemos la distancia del player a este agente.
            float distance = (transform.position - player.transform.position).magnitude;
            if (distance < shortestDistance)
            {
                // pues ahora la distancia más corta es esta distancia
                shortestDistance = distance;
                nearestPlayer = player; // actualizamos la referencia al player que está más cercano.
            }
        }
        
        // cuando termine el foreach, ya vamos a tener al player más cercano a este agente.
        // EXCEPTO si no hay ningún player.
        if (nearestPlayer)
        {
            // Entonces sí encontramos al player má cercano. Aquí ya podemos reaccionar a eso.
            _steeringBehaviors.SetTarget(nearestPlayer.transform.position);
        }
        // si no no hay quien reaccionar.
        _steeringBehaviors.RemoveTarget();
        
    }
}
