using UnityEngine;

public class IceBolt : AbilityCard
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float damage = 5;
    [SerializeField] private float freezeTime = 2f;

    public override void SkillAction()
    {
        GameObject throwedProj = Instantiate(projectile, playerManager.MovementScript.transform.position, Quaternion.identity);

        if (throwedProj.TryGetComponent<Projectile_IceBolt>(out Projectile_IceBolt projectile_Icebolt))
        {
            projectile_Icebolt.damage = damage;
            projectile_Icebolt.freezeTime = freezeTime;
            projectile_Icebolt.facingDir = playerManager.facingDir;
        }
        else Debug.Log("Projectile does not contain specific projectile logic");
    }
}
