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
        // 1. 모든 아이템 버튼 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        int[] selectionSlots = new int[3];                  // 선택지 배열
        List<Item> selectableAbilities = new List<Item>();  // 선택 가능한 어빌리티 배열

        // 2. 인벤토리 상태 확인
        bool isInventoryFull = Inventory.Instance.index >= Inventory.Instance.slots.Count;

        // 2.1 선택 가능한 어빌리티 추가
        foreach (Item item in items)
        {
            if (isInventoryFull)
            {
                // 인벤토리가 가득 찼을 때는 만렙이 아닌 어빌리티 중 현재 보유 중인 것만 추가
                if (item.level < item.data.damages.Length && Inventory.Instance.HasItem(item))
                {
                    selectableAbilities.Add(item);
                }
            }
            else
            {
                // 인벤토리가 가득 차지 않은 경우, 만렙이 아닌 어빌리티와 힐 아이템 추가
                if (item.level < item.data.damages.Length || item.data.itemType == ItemData.ItemType.Heal)
                {
                    selectableAbilities.Add(item);
                }
            }
        }

        // 3. 모든 어빌리티가 만렙일 경우, 힐 아이템만 추가
        if (selectableAbilities.Count == 0)
        {
            selectableAbilities.Add(items[items.Length - 1]); // 힐 아이템 (항상 마지막에 위치)
        }

        // 4. 랜덤으로 3개의 어빌리티 선택하여 활성화
        int maxSelections = 3;
        for (int i = 0; i < maxSelections; i++)
        {
            // 선택 가능한 어빌리티가 없으면 종료
            if (selectableAbilities.Count == 0)
                break;

            // 선택 가능한 어빌리티 리스트 중 하나를 랜덤하게 선택
            int randomIndex = Random.Range(0, selectableAbilities.Count);
            Item selectedItem = selectableAbilities[randomIndex];

            // 선택된 어빌리티 활성화
            selectedItem.gameObject.SetActive(true);

            // 활성화된 어빌리티는 리스트에서 제거하여 중복 방지
            selectableAbilities.RemoveAt(randomIndex);
        }
    }
}