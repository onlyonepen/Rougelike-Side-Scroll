using UnityEngine;

public class BackStab : AbilityCard
{
    public float Range = 2f;
    public float Damage = 5f;
    public float CritMult = 2f;
    public float StaggeringTime = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 castOrigin;
    private Vector2 castSize = new Vector2(1, 0.5f);
    private float castAngle = 0f;

    public override void SkillAction()
    {
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
                    if (enemyClass.FacingDir() == playerManager.facingDir)
                    {
                        totalDmg *= CritMult;
                        _isCrit = true;
                    }
                }
                hit.gameObject.GetComponent<EnemyClass>().TakeDamage(totalDmg, StaggeringTime, _isCrit);
            }

            hitEnemy = true;
        }

        if (hitEnemy)
        {

        }
    }
}
