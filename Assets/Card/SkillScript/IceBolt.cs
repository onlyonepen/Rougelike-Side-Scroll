using UnityEngine;
using System.Collections;

public class IceBolt : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 1f; } }

    [SerializeField] private GameObject projectile;
    [SerializeField] private float damage = 5;
    [SerializeField] private float freezeTime = 2f;

    public override void UseAbility(PlayerManagerScript playerManager)
    {
        StartCoroutine(Ability(playerManager));
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        yield return new WaitForSeconds(Anticipation);

        GameObject throwedProj = Instantiate(projectile, playerManager.MovementScript.transform.position, Quaternion.identity);

        if (throwedProj.TryGetComponent<Projectile_IceBolt>(out Projectile_IceBolt projectile_Icebolt))
        {
            projectile_Icebolt.damage = damage;
            projectile_Icebolt.freezeTime = freezeTime;
            projectile_Icebolt.facingDir = playerManager.facingDir;
        }
        else Debug.Log("Projectile does not contain specific projectile logic");


        yield return new WaitForSeconds(Recovery);
    }
}
