using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SharpShotter : AbilityCard
{
    public override float Anticipation { get { return 0.5f; } }
    public override float Recovery { get { return 1f; } }

    [SerializeField] private Material lineMat;
    [SerializeField] private float range = 3f;
    [SerializeField] private float damage = 5;
    [SerializeField] private float critFac = 2;
    [SerializeField] private float staggeringTime = 0.2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;

    public override void UseAbility(PlayerManagerScript playerManager)
    {
        StartCoroutine(Ability(playerManager));
    }

    public IEnumerator Ability(PlayerManagerScript playerManager)
    {
        yield return new WaitForSeconds(Anticipation);

        List<GameObject> allLineObj = new List<GameObject>();

        Vector3 playerPos = playerManager.MovementScript.transform.position;
        Collider2D[] nearbyEnemy = Physics2D.OverlapCircleAll(playerPos, range, enemyLayer);
        foreach (Collider2D enemy in nearbyEnemy)
        {
            Vector2 direction = enemy.transform.position - playerPos;
            bool isObstructed = Physics2D.Raycast(playerPos, direction, direction.magnitude, groundLayer);
            if (!isObstructed)
            {
                EnemyClass eSM = enemy.gameObject.GetComponent<EnemyClass>();
                float totalDmg = damage;
                if (!eSM.GroundCheck())
                {
                    totalDmg = damage * critFac;
                }
                eSM.TakeDamage(totalDmg, staggeringTime);
                allLineObj.Add(spawnLineRenderer(playerPos, eSM.transform.position));
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (GameObject obj in allLineObj)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        allLineObj.Clear();

        yield return new WaitForSeconds(Recovery - 0.1f);
    }

    private GameObject spawnLineRenderer(Vector2 pointA, Vector2 pointB)
    {
        GameObject lineObject = new GameObject("MyLineObject");

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.material = lineMat;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2; // Set number of points
        lineRenderer.SetPosition(0, pointA); // First point at origin
        lineRenderer.SetPosition(1, pointB); // Second point 5 units up

        return lineObject;
    }
}
