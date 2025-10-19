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

    private void decidedCard(AbilityCard card)
    {
        isSelecting = true;
        PlayerManagerScript player = PlayerManagerScript.Instance;

        //add the card to the front;
        player.HandManager.PutBackToDeck(card.gameObject);
        
        player.HandManager.UsedCard -= decidedCard;
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        Debug.Log("Use" + this.ToString());
        yield return new WaitForSeconds(Anticipation);
        playerManager.HandManager.UsedCard += decidedCard;

        isSelecting = false;
        Time.timeScale = 0;

        yield return new WaitUntil(() => isSelecting);
    }
}
