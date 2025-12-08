using UnityEngine;
using System.Collections;
public class Reserve : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 0.2f; } }

    bool isSelecting = false;



    public override IEnumerator Ability(PlayerManagerScript playerManager)
    {
        Debug.Log("Use" + this.ToString());
        yield return new WaitForSeconds(Anticipation);


        yield return new WaitUntil(() => !isSelecting);
    }
}
