using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        StageManager.instance.GameStop();
        AudioManager.instance.PlaySfx(AudioManager.SFX.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        StageManager.instance.GameResume();
        AudioManager.instance.PlaySfx(AudioManager.SFX.Select);
        AudioManager.instance.EffectBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // 1. ��� ������ ��ư ��Ȱ��ȭ
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        int[] selectionSlots = new int[3];                  // ������ �迭
        List<Item> selectableAbilities = new List<Item>();  // ���� ������ �����Ƽ �迭

        // 2. �κ��丮 ���� Ȯ��
        bool isInventoryFull = Inventory.Instance.index >= Inventory.Instance.slots.Count;

        // 2.1 ���� ������ �����Ƽ �߰�
        foreach (Item item in items)
        {
            if (isInventoryFull)
            {
                // �κ��丮�� ���� á�� ���� ������ �ƴ� �����Ƽ �� ���� ���� ���� �͸� �߰�
                if (item.level < item.data.damages.Length && Inventory.Instance.HasItem(item))
                {
                    selectableAbilities.Add(item);
                }
            }
            else
            {
                // �κ��丮�� ���� ���� ���� ���, ������ �ƴ� �����Ƽ�� �� ������ �߰�
                if (item.level < item.data.damages.Length || item.data.itemType == ItemData.ItemType.Heal)
                {
                    selectableAbilities.Add(item);
                }
            }
        }

        // 3. ��� �����Ƽ�� ������ ���, �� �����۸� �߰�
        if (selectableAbilities.Count == 0)
        {
            selectableAbilities.Add(items[items.Length - 1]); // �� ������ (�׻� �������� ��ġ)
        }

        // 4. �������� 3���� �����Ƽ �����Ͽ� Ȱ��ȭ
        int maxSelections = 3;
        for (int i = 0; i < maxSelections; i++)
        {
            // ���� ������ �����Ƽ�� ������ ����
            if (selectableAbilities.Count == 0)
                break;

            // ���� ������ �����Ƽ ����Ʈ �� �ϳ��� �����ϰ� ����
            int randomIndex = Random.Range(0, selectableAbilities.Count);
            Item selectedItem = selectableAbilities[randomIndex];

            // ���õ� �����Ƽ Ȱ��ȭ
            selectedItem.gameObject.SetActive(true);

            // Ȱ��ȭ�� �����Ƽ�� ����Ʈ���� �����Ͽ� �ߺ� ����
            selectableAbilities.RemoveAt(randomIndex);
        }
    }
}