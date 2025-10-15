using System.Collections;
using UnityEngine;

public class FireArrow : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 1f; } }

    [SerializeField] private GameObject projectile;
    [SerializeField] private float damage = 5;
    [SerializeField] private float staggeringTime = 0.2f;
    [SerializeField] private Vector2 throwVec = new Vector2(20, 5);

    public override void UseAbility(PlayerManagerScript playerManager)
    {
        StartCoroutine(Ability(playerManager));
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        yield return new WaitForSeconds(Anticipation);

        GameObject throwedProj = Instantiate(projectile, playerManager.MovementScript.transform.position, Quaternion.identity);

        if (throwedProj.TryGetComponent<Projectile_FireArrow>(out Projectile_FireArrow projectile_Basic))
        {
            projectile_Basic.damage = damage;
            projectile_Basic.staggeringTime = staggeringTime;
        }
        else Debug.Log("Projectile does not contain specific projectile logic");

        Vector2 _vec = new Vector2(throwVec.x * playerManager.facingDir, throwVec.y);
        throwedProj.GetComponent<Rigidbody2D>().AddForce(_vec, ForceMode2D.Impulse);

        yield return new WaitForSeconds(Recovery);
    }
}
