using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxVisualizeUtils : MonoBehaviour
{
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private Sprite circleSprite;

    private List<GameObject> boxPool = new List<GameObject>();
    private List<GameObject> circlePool = new List<GameObject>();

    private int lastBoxUsed = -1;
    private int lastCircleUsed = -1;

    public static HitboxVisualizeUtils Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        for (int i = 0; i < 5; i++)
        {
            createBoxPool(i);
            createCirclePool(i);
        }
    }

    public Collider2D[] OverlapBoxWithVisualize(Vector2 point, Vector2 size, float angle, int layerMask = Physics.DefaultRaycastLayers)
    {
        GameObject box = getVisualizeBox();
        box.SetActive(true);
        box.transform.localScale = size;
        box.transform.localPosition = point;
        StartCoroutine(delayDisable(box));
        return Physics2D.OverlapBoxAll(point, size, angle, layerMask);
    }

    public Collider2D[] OverlapCircleWithVisualize(Vector2 point, float radius, int layerMask = Physics.DefaultRaycastLayers)
    {
        GameObject circle = getVisualizeCircle();
        circle.SetActive(true);
        circle.transform.localScale = Vector2.one * radius * 2;
        circle.transform.position = point;
        StartCoroutine(delayDisable(circle));
        return Physics2D.OverlapCircleAll(point, radius, layerMask);
    }

    private GameObject getVisualizeBox()
    {
        lastBoxUsed++;
        lastBoxUsed %= boxPool.Count;

        return boxPool[lastBoxUsed];
    }

    private GameObject getVisualizeCircle()
    {
        lastCircleUsed++;
        lastCircleUsed %= circlePool.Count;

        return circlePool[lastCircleUsed];
    }

    IEnumerator delayDisable(GameObject _go)
    {
        yield return new WaitForSeconds(0.5f);
        _go.SetActive(false);
    }

    #region Pooling
    private void createBoxPool(int i)
    {
        GameObject _go = new GameObject("Visual Box " + i);
        boxPool.Add(_go);
        _go.transform.parent = transform;
        SpriteRenderer _sprite = _go.AddComponent<SpriteRenderer>();
        _sprite.sprite = squareSprite;
        _sprite.color = Color.blue;
        _go.SetActive(false);
    }
    private void createCirclePool(int i)
    {
        GameObject _go = new GameObject("Visual Circle " + i);
        circlePool.Add(_go);
        _go.transform.parent = transform;
        SpriteRenderer _sprite = _go.AddComponent<SpriteRenderer>();
        _sprite.sprite = circleSprite;
        _sprite.color = Color.blue;
        _go.SetActive(false);
    }
    #endregion
}
