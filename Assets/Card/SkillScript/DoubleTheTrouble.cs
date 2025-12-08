using System.Collections;
using UnityEngine;

public class DoubleTheTrouble : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 0.2f; } }
    public float Range = 2f;
    public float Damage = 5f;
    public float CritMult = 2f;
    public float StaggeringTime = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 castOrigin;
    private Vector2 castSize = new Vector2(1, 1f);
    private float castAngle = 0f;


    public override IEnumerator Ability(PlayerManagerScript playerManager)
    {
        Debug.Log("UseSkill");
        yield return new WaitForSeconds(Anticipation);

        Collider2D[] castHit;
        Vector3 playerPos = playerManager.MovementScript.transform.position;
        Vector3 attackOffset = Vector3.right * playerManager.facingDir * Range / 2;

        castOrigin = attackOffset + playerPos;

        castHit = HitboxVisualizeUtils.Instance.OverlapBoxWithVisualize(castOrigin, castSize, castAngle, enemyLayer);

        bool hitEnemy = false;

        foreach (Collider2D hit in castHit)
        {
            bool _isCrit = false;
            Vector2 direction = hit.transform.position - playerPos;
            bool isObstructed = Physics2D.Raycast(playerPos, direction, direction.magnitude, groundLayer);
            if (!isObstructed)
            {
                float totalDmg = Damage;
                if (hit.transform.TryGetComponent<EnemyClass>(out EnemyClass enemyClass))
                {
                    if (enemyClass.isMarkedForTomorrow)
                    {
                        totalDmg *= CritMult;
                        _isCrit = true;
                    }
                }
                hit.gameObject.GetComponent<EnemyClass>().TakeDamage(totalDmg, StaggeringTime, _isCrit);
                MarkForTomorrow _mFT = new MarkForTomorrow();
                enemyClass.mFTInstance = _mFT;
                enemyClass.ApplyStatusEffect(_mFT);
            }

            hitEnemy = true;
        }

        if (hitEnemy)
        {

        }

        yield return new WaitForSeconds(Recovery);
    }
}
