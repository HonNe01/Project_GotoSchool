using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{ 
    public static Inventory Instance;

    public List<Image> slots; // Item 0~5 �̹��� ������Ʈ ����Ʈ

    public int index = 0;

    private void Awake()
    {
        Instance = this;
    }

    // ��ų �������� ȹ���� ���Կ� �߰��ϴ� �޼���
    public void AddSkill(Sprite Icon)
    {
        if (index < 0 || index >= slots.Count)
        {
            Debug.LogWarning("No available slot to add skill");
            return;
        }

        slots[index].sprite = Icon;
        index++;
    }

    public bool HasItem(Item item)
    {
        foreach (Image slot in slots)
        {
            if (slot.sprite == item.data.itemIcon)
            {
                return true;
            }
        }
        return false;
    }
}