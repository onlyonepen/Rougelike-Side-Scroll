
using System.Collections;
using UnityEngine;

public class BasicAttack : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 1f; } }
    public float Range = 2f;
    public float Damage = 5f;
    public float StaggeringTime = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 castOrigin;
    private Vector2 castSize = new Vector2(1, 1);
    private float castAngle = 0f;
    private Vector2 castDirection = Vector2.down;
    private float castDistance = 0.1f;
    private bool isAttacking = false;

    public override void UseAbility(PlayerManagerScript playerManager)
    {
        StartCoroutine(Ability(playerManager));
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        Debug.Log("UseSkill");
        yield return new WaitForSeconds(Anticipation);

        isAttacking = true;
        Collider2D[] castHit;
        Vector3 playerPos = playerManager.MovementScript.transform.position;
        Vector3 attackOffset = Vector3.right * playerManager.facingDir * Range / 2;

        castOrigin = attackOffset + playerPos;

        castHit = Physics2D.OverlapBoxAll(castOrigin, castSize, castAngle, enemyLayer);

        bool hitEnemy = false;

        foreach (Collider2D hit in castHit)
        {
            Vector2 direction = hit.transform.position - playerPos;
            bool isObstructed = Physics2D.Raycast(playerPos, direction, direction.magnitude, groundLayer);
            if (!isObstructed)
            {
                hit.gameObject.GetComponent<EnemyClass>().TakeDamage(Damage, StaggeringTime);
            }

            hitEnemy = true;
        }



        if (hitEnemy)
        {

        }

        yield return new WaitForSeconds(Recovery);

        isAttacking = false;
    }

    //private void OnDrawGizmos()
    //{
    //    if (isAttacking)
    //    {
    //        Gizmos.color = Color.red;

    //        if (hitEnemy)
    //        {
    //            Gizmos.color = Color.green;
    //        }

    //        float totalDistance = castDistance;
    //        Gizmos.DrawWireCube(castOrigin, castSize);
    //    }
    //}
}

//using System.Collections;
//using UnityEngine;

//public class BasicAttack : AbilityCard
//{
//    public override float Anticipation { get { return 0.5f; } }
//    public override float Recovery { get { return 1f; } }

//    public float range = 2f;

//    public float damage = 5f;

//    private bool attacking = false;

//    public override void UseAbility(int facingDir, Vector3 playerPos)
//    {
//        StartCoroutine(Ability(facingDir, playerPos));
//    }

//    public IEnumerator Ability(int facingDir, Vector3 playerPos)
//    {
//        attacking = true;
//        Debug.Log("UseSkill");
//        yield return new WaitForSeconds(Anticipation);

//        RaycastHit2D[] castHit;
//        bool HitEnemy = false;
//        Vector3 attackOffset = Vector3.right * facingDir * range / 2;
//        castHit = Physics2D.BoxCastAll(attackOffset + playerPos, new Vector3(1, 1), 0f, Vector2.down, 0.1f, Physics2D.AllLayers);

//        foreach (RaycastHit2D hit in castHit)
//        {
//            if (hit.transform.TryGetComponent<IDamagable>(out IDamagable damagable))
//            {
//                damagable.TakeDamage(damage);
//            }

//            HitEnemy = true;
//        }

//        if (HitEnemy)
//        {

//        }

//        yield return new WaitForSeconds(Recovery);

//        attacking = false;
//    }
//}