using TMPro;
using UnityEngine;

public class LatestDamageDealt : MonoBehaviour
{
    TextMeshProUGUI text;
    public static LatestDamageDealt Instance { get; private set; }
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateDamage(float dmg, bool isCrit)
    {
        string dmgText = dmg + " DMG";
        if (isCrit) dmgText += "!";
        text.text = dmgText;
    }
}
