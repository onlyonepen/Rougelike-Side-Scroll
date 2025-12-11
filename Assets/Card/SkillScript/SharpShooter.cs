using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SharpShotter : AbilityCard
{
    [SerializeField] private Material lineMat;
    [SerializeField] private float range = 3f;
    [SerializeField] private float damage = 5;
    [SerializeField] private float critFac = 2;
    [SerializeField] private float staggeringTime = 0.2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask groundLayer;
    private List<GameObject> allLineObj;
    public override void SkillAction()
    {
        allLineObj = new List<GameObject>();

        Vector3 playerPos = playerManager.MovementScript.transform.position;
        Collider2D[] nearbyEnemy = Physics2D.OverlapCircleAll(playerPos, range, enemyLayer);
        foreach (Collider2D enemy in nearbyEnemy)
        {
            bool _isCrit = false;
            Vector2 direction = enemy.transform.position - playerPos;
            bool isObstructed = Physics2D.Raycast(playerPos, direction, direction.magnitude, groundLayer);
            if (!isObstructed)
            {
                EnemyClass eSM = enemy.gameObject.GetComponent<EnemyClass>();
                float totalDmg = damage;
                if (!eSM.GroundCheck())
                {
                    totalDmg = damage * critFac;
                    _isCrit = true;
                }
                eSM.TakeDamage(totalDmg, staggeringTime, _isCrit);
                allLineObj.Add(spawnLineRenderer(playerPos, eSM.transform.position));
            }
        }

        Invoke(nameof(deleteLine), 0.1f);
    }

    private void deleteLine()
    {
        foreach (GameObject obj in allLineObj)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        allLineObj.Clear();
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
