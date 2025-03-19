using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public WeaponManager weapon;
    public Buff buff;

    Image icon;
    TextMeshProUGUI textLevel;
    TextMeshProUGUI textName;
    TextMeshProUGUI textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    void OnEnable()
    {
        if (level < data.damages.Length)
        {
            if (level == 0)
            {
                textLevel.text = " <color=yellow>NEW!</color>";
            }
            else if (level == data.damages.Length - 1)
            {
                textLevel.text = " <color=green>Master!</color>";
            }
            else
            {
                textLevel.text = "Lv." + (level + 1);
            }

            textDesc.text = string.Format(data.itemDesc);

            /*
            switch (data.itemType)
            {
                case ItemData.ItemType.Melee:
                case ItemData.ItemType.Range:
                    if (level < data.damages.Length && level < data.counts.Length)
                    {
                        textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                    }
                    else
                    {
                        textDesc.text = data.itemDesc;
                    }
                    break;
                case ItemData.ItemType.Buff:
                    textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                    break;
                default:
                    textDesc.text = string.Format(data.itemDesc);
                    break;
            }
            */
        }
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    // 새로운 어빌리티 획득
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<WeaponManager>();
                    weapon.Init(data);

                    // 인벤토리에 추가
                    Inventory.Instance.AddSkill(data.itemIcon);

                    Debug.Log("새로운 어빌리티 획득! : " + data.itemName);
                }
                else
                {
                    // 어빌리티 레벨업
                    float nextDamage = data.damages[level - 1];
                    float nextCount = 0;
                    nextCount += data.counts[level - 1];

                    weapon.LevelUp(nextDamage, nextCount);

                    Debug.Log("어빌리티 레벨업! : " + data.itemName + " Lv. " + level);
                }

                level++;
                break;
            case ItemData.ItemType.Buff:
                if (level == 0)
                {
                    // 새로운 어빌리티 획득
                    GameObject newBuff = new GameObject();
                    buff = newBuff.AddComponent<Buff>();
                    buff.Init(data);

                    // 인벤토리에 추가
                    Inventory.Instance.AddSkill(data.itemIcon);

                    Debug.Log("새로운 어빌리티 획득! : " + data.itemName);
                }
                else
                {
                    // 어빌리티 레벨업
                    float damage = data.damages[level - 1];
                    float count = data.counts[level - 1];
                    buff.LevelUp(damage, count);

                    Debug.Log("어빌리티 레벨업! : " + data.itemName + " Lv. " + level);
                }

                level++;
                break;
            case ItemData.ItemType.Heal:
                // 체력 회복
                PlayerManager.instance.Heal();

                Debug.Log("체력 회복!");
                break;
        }

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}