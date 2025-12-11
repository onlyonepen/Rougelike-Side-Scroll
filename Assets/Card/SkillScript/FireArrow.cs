using System.Collections;
using UnityEngine;

public class FireArrow : AbilityCard
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float damage = 5;
    [SerializeField] private float staggeringTime = 0.2f;
    [SerializeField] private Vector2 throwVec = new Vector2(20, 5);

    public override void SkillAction()
    {
        GameObject throwedProj = Instantiate(projectile, playerManager.MovementScript.transform.position, Quaternion.identity);

        if (throwedProj.TryGetComponent<Projectile_FireArrow>(out Projectile_FireArrow projectile_Basic))
        {
            projectile_Basic.damage = damage;
            projectile_Basic.staggeringTime = staggeringTime;
        }
        else Debug.Log("Projectile does not contain specific projectile logic");

        Vector2 _vec = new Vector2(throwVec.x * playerManager.facingDir, throwVec.y);
        throwedProj.GetComponent<Rigidbody2D>().AddForce(_vec, ForceMode2D.Impulse);
    }
}
