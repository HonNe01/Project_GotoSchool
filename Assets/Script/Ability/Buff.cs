using UnityEngine;

public class Buff : MonoBehaviour
{
    public float posRate;
    public float negRate;

    public int id;

    public void Init(ItemData data)     // ���� ����
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
                case 0: // ���� - ������ ����
                    PlayerManager.instance.buffMultiplier += buff.posRate;
                    break;
                case 1: // �ʷϺ� - ������ ���� & �̵��ӵ� ����
                    PlayerManager.instance.buffMultiplier += buff.posRate;
                    PlayerManager.instance.player.bonusSpeed -= buff.negRate;
                    break;
                case 2: // ���������� - �̵��ӵ� ����
                    PlayerManager.instance.player.bonusSpeed += buff.posRate;
                    break;
                case 3: // ������ �帵ũ - ������ ���� & �ִ� ü�� ����
                    PlayerManager.instance.buffMultiplier += buff.posRate;
                    PlayerManager.instance.bonusHealth -= buff.negRate;

                    // ���� ü�� ����
                    if (PlayerManager.instance.health > PlayerManager.instance.MaxHealth)
                    {
                        PlayerManager.instance.health = PlayerManager.instance.MaxHealth;
                    }
                    break;
                case 4: // �̿� ���� - ���ݼӵ� ����
                    break;
            }

            Debug.Log(buff.name + "���� ������ : " + buff.posRate + " | " + PlayerManager.instance.buffMultiplier);
        }

        WeaponManager[] weapons = transform.parent.GetComponentsInChildren<WeaponManager>();
        foreach (WeaponManager weapon in weapons)
        {
            Debug.Log(weapon.name + "�� ���� ������");

            weapon.ApplyDamage();
        }
    }
}