using System.Collections;
using TMPro;
using UnityEngine;

public class CharStatInfo : MonoBehaviour
{
    [Header(" # Stat")]
    public GameObject[] powerBars;      // Damage
    public GameObject[] speedBars;      // Speed
    public GameObject[] abilityBars;    // Weapon Speed, Rate, Count

    public float statTime = 0.1f;

    [Header(" # Info")]
    public string[] nameTexts;
    public string[] infoTexts;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI infoText;

    public float infoTime = 0.05f;

    private void OnEnable()
    {
        ResetUI();

        StartCoroutine(SetPower(Character.Damage));
        StartCoroutine(SetSpeed(Character.Speed));
        StartCoroutine(SetAbility());

        EnterText(GameManager.instance.playerId);
    }

    IEnumerator SetPower(float value)
    {
        int barCount = (value > 1f) ? 5 : 3;
        for (int i = 0; i < powerBars.Length; i++)
        {
            powerBars[i].SetActive(i < barCount);

            yield return new WaitForSeconds(statTime);
        }
    }

    IEnumerator SetSpeed(float value)
    {
        int barCount = (value > 1f) ? 5 : 3;
        for (int i = 0; i < speedBars.Length; i++)
        {
            speedBars[i].SetActive(i < barCount);

            yield return new WaitForSeconds(statTime);
        }
    }

    IEnumerator SetAbility()
    {
        float maxAbilityValue = Mathf.Max(Character.WeaponSpeed, Character.WeaponRate, Character.Count);
        int barCount = (maxAbilityValue > 1f) ? 5 : 3;
        for (int i = 0; i < abilityBars.Length; i++)
        {
            abilityBars[i].SetActive(i < barCount);

            yield return new WaitForSeconds(statTime);
        }
    }

    void ResetUI()
    {
        foreach (GameObject bar in powerBars)
        {
            bar.SetActive(false);
        }
        foreach (GameObject bar in speedBars)
        {
            bar.SetActive(false);
        }
        foreach (GameObject bar in abilityBars)
        {
            bar.SetActive(false);
        }

        nameText.text = string.Empty;
        infoText.text = string.Empty;
    }

    public void EnterText(int playerId)
    {
        StartCoroutine(Typing(nameText, nameTexts[playerId]));
        StartCoroutine(Typing(infoText, infoTexts[playerId]));
    }

    IEnumerator Typing(TextMeshProUGUI text, string enter)
    {
        text.text = string.Empty;

        for(int i = 0; i < enter.Length; i++)
        {
            text.text += enter[i];

            yield return new WaitForSeconds(infoTime);
        }
    }
}
