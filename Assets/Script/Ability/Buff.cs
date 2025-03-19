using UnityEngine;

public class Buff : MonoBehaviour
{
    public float posRate;
    public float negRate;

    public int id;

    public void Init(ItemData data)     // 버프 생성
    {
        // Basic Set
        name = "Buff " + data.itemName;
        transform.parent = PlayerManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        posRate = data.baseDamage;
        negRate = data.baseCount;


        ApplyBuff();
    }

    public void LevelUp(float positive, float negative)
    {
        this.posRate = positive;
        this.negRate = negative;

        
        ApplyBuff();
    }

    public void ApplyBuff()
    {
        PlayerManager.instance.bonusHealth = 0f;
        PlayerManager.instance.buffMultiplier = 0f;
        PlayerManager.instance.player.bonusSpeed = 0f;

        Buff[] buffs = transform.parent.GetComponentsInChildren<Buff>();
        foreach (Buff buff in buffs)
        {
            switch (buff.id)
            {
                case 0: // 족보 - 데미지 증가
                    PlayerManager.instance.buffMultiplier += buff.posRate;
                    break;
                case 1: // 초록병 - 데미지 증가 & 이동속도 감소
                    PlayerManager.instance.buffMultiplier += buff.posRate;
                    PlayerManager.instance.player.bonusSpeed -= buff.negRate;
                    break;
                case 2: // 에어조나단 - 이동속도 증가
                    PlayerManager.instance.player.bonusSpeed += buff.posRate;
                    break;
                case 3: // 에너지 드링크 - 데미지 증가 & 최대 체력 감소
                    PlayerManager.instance.buffMultiplier += buff.posRate;
                    PlayerManager.instance.bonusHealth -= buff.negRate;

                    // 현재 체력 조정
                    if (PlayerManager.instance.health > PlayerManager.instance.MaxHealth)
                    {
                        PlayerManager.instance.health = PlayerManager.instance.MaxHealth;
                    }
                    break;
                case 4: // 이온 음료 - 공격속도 증가
                    break;
            }

            Debug.Log(buff.name + "버프 적용중 : " + buff.posRate + " | " + PlayerManager.instance.buffMultiplier);
        }

        WeaponManager[] weapons = transform.parent.GetComponentsInChildren<WeaponManager>();
        foreach (WeaponManager weapon in weapons)
        {
            Debug.Log(weapon.name + "에 버프 적용중");

            weapon.ApplyDamage();
        }
    }
}