using UnityEngine;


public enum ESwordsmanAttacks
{
    None,
    BasicSlashAttack,
    AreaSlashAttack,
}

public class SwordsmanEnemy : BaseEnemy
{
    [SerializeField] protected Collider swordCollider;

    // Esta función se va a mandar a llamar por el animation event y nos dice qué acción del 
    // swordsman está realizando
    void BeginAttackActiveFrames(ESwordsmanAttacks attackID)
    {
        // Envía que sucedió este event
        Debug.Log($"Empezó los frames activos del ataque: {attackID}");
        // el collider de la espada se activa
        swordCollider.gameObject.SetActive(true);
    }
    
    void EndAttackActiveFrames(ESwordsmanAttacks attackID)
    {
        // Envía que sucedió este event
        Debug.Log($"Terminaron los frames activos del ataque: {attackID}");
        // el collider de la espada se desactiva
        swordCollider.gameObject.SetActive(false);
    }

    void FinishedAttack(ESwordsmanAttacks attackID)
    {
        Debug.Log($"Terminó la animación del ataque: {attackID}");
    }
    

}
