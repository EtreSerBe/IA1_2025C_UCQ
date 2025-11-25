using UnityEngine;

[CreateAssetMenu(fileName = "SO_AttackParams", menuName = "Attacks/Attack Parameters")]
public class SO_AttackParameters : ScriptableObject
{
    // Daño que hace
    [SerializeField] protected int damage = 1;
    public int Damage => damage;
    
    // Cadencia de este ataque (cada cuánto tiempo se puede realizar), en otras palabras es su Cooldown.
    [SerializeField] protected float cooldown = 0.5f;
    public float Cooldown => cooldown;
    
    [SerializeField] protected float range = 3.0f;
    public float Range => range;

}
