using UnityEngine;

public abstract class AbilityCard : MonoBehaviour
{
    public abstract float Anticipation { get; }
    public abstract float Recovery { get; }

    public abstract void UseAbility(PlayerManagerScript playerManager);
}
