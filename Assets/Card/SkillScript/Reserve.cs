using UnityEngine;
using System.Collections;
public class Reserve : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 1f; } }

    bool isSelecting = false;

    public override void UseAbility(PlayerManagerScript playerManager)
    {
        StartCoroutine(Ability(playerManager));
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        Debug.Log("Use" + this.ToString());
        yield return new WaitForSeconds(Anticipation);

        //HandStateManager handManager = playerManager.HandManager;
        //handManager.ChangeState(handManager.ReserveState);

        yield return new WaitUntil(() => !isSelecting);
    }
}
