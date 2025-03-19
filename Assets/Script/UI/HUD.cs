using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // UI 종류
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    // UI 오브젝트
    TextMeshProUGUI myText;
    Slider mySlider;

    // Exp Info
    [HideInInspector] public float curExp;
    [HideInInspector] public float maxExp;

    // Health Info
    [HideInInspector] public float curHealth;
    [HideInInspector] public float maxHealth;

    void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch (type) {
            case InfoType.Exp:
                curExp = PlayerManager.instance.exp;
                maxExp = PlayerManager.instance.maxExp[Mathf.Min(PlayerManager.instance.level, PlayerManager.instance.maxExp.Length - 1) - 1];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", PlayerManager.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", PlayerManager.instance.kill);
                break;
            case InfoType.Time:
                float remainTime = StageManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                float curHealth = PlayerManager.instance.health;
                float maxHealth = PlayerManager.instance.MaxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
