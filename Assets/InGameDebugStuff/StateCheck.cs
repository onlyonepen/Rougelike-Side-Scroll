using TMPro;
using UnityEngine;

public class StateCheck : MonoBehaviour
{
    public EnemyClass enemy;
    public TextMeshProUGUI text;

    private void Start()
    {
        enemy.OnChangeStateDebug += RefreshDebugText;
    }

    private void OnDisable()
    {
        enemy.OnChangeStateDebug -= RefreshDebugText;
    }

    public void RefreshDebugText(string state)
    {
        text.text = state;
    }
}
