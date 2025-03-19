using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public ItemData.ItemType type;
    public int id;
    public int prefabId;
    public float baseDamage;
    public float damage;
    public float count;
    public float speed;

    float timer;

    Hand hands;
    Player player;
    CircleCollider2D circleCollider;

    void Awake()
    {
        player = PlayerManager.instance.player;
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (!StageManager.instance.isGameRunning)
            return;

        if (type == ItemData.ItemType.Melee)
        {
            switch (id)
            {
                case 0:
                    transform.Rotate(Vector3.back * speed * 0.5f * Time.deltaTime);

                    break;  // 연필
                case 1:
                    transform.Rotate(Vector3.forward * speed * 0.5f * Time.deltaTime);

                    break;  // 꼬질꼬질
                default:

                    break;
            }
        }

        else if (type == ItemData.ItemType.Range)
        {
            switch (id)
            {
                case 0:
                    timer += Time.deltaTime;

                    if (timer > speed)
                    {
                        timer = 0f;
                        Book();
                    }
                    break;  // 전공책
                default:

                    break;
            }
        }
    }

    public void LevelUp(float damage, float count)
    {
        // 무기 능력치 증가
        baseDamage = damage;
        this.count += count;

        this.damage = CalcDamage();

        switch (type)
        {
            case ItemData.ItemType.Melee:
                if (id == 0) Pencil();
                else if (id == 1) Dirty();

                break;
            case ItemData.ItemType.Range:
                break;
        }

        
        Buff buff = PlayerManager.instance.player.GetComponentInChildren<Buff>();
        if (buff != null)
        {
            buff.ApplyBuff();
        }
    }

    public void Init(ItemData data)     // 무기 생성
    {
        // Basic Skill Set
        name = "Weapon " + data.itemName;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        type = data.itemType;
        baseDamage = data.baseDamage;
        count = data.baseCount + Character.Count;

        damage = CalcDamage();

        for (int index = 0; index < StageManager.instance.poolManager.prefabs.Length; index++) {
            if (data.projectile == StageManager.instance.poolManager.prefabs[index]) {
                prefabId = index;
                break;
            }
        }

        if (data.itemType == ItemData.ItemType.Melee)
        {
            switch (id)
            {
                case 0:     // 연필
                    speed = 100 * Character.WeaponSpeed;
                    Pencil();

                    break;
                case 1:     // 꼬질꼬질
                    Transform bullet = StageManager.instance.poolManager.Get(prefabId).transform;
                    bullet.parent = transform;

                    // 위치, 각도 초기화
                    bullet.localPosition = Vector3.zero;
                    bullet.localRotation = Quaternion.identity;
                    speed = 100;

                    Dirty();
                    break;
            }
        }

        else if (data.itemType == ItemData.ItemType.Range)
        {
            switch (id)
            {
                case 0:     // 전공책
                    speed = 0.5f * Character.WeaponRate;

                    break;
            }
        }
        // Hand Set
        InHand(data);
        Buff buff = PlayerManager.instance.player.GetComponentInChildren<Buff>();
        if (buff != null)
        {
            buff.ApplyBuff();
        }
    }

    void InHand(ItemData data)          // 무기 장착
    {
        Hand hand = null;

        // 무기 타입과 itemId에 따라 해당 부위 결정
        if (data.itemType == ItemData.ItemType.Melee)
        {
            hand = player.hands[(int)Hand.HandType.Left];
        }
        else if (data.itemType == ItemData.ItemType.Range)
        {
            // Range, 0 (전공책)은 Back, 나머지는 Right
            hand = (data.itemId == 0) ? player.hands[(int)Hand.HandType.Back] : player.hands[(int)Hand.HandType.Right];
        }

        // 해당 부위가 비어있는 경우만 설정
        if (hand != null && hand.spriter.sprite == null)
        {
            hand.spriter.sprite = data.hand;
            hand.gameObject.SetActive(true);
        }
    }

    public float CalcDamage()
    {
        return baseDamage * Character.Damage;
    }

    public void ApplyDamage()
    {
        damage = baseDamage + (baseDamage * PlayerManager.instance.buffMultiplier) + (baseDamage * (1 - Character.Damage));
    }

    void Pencil()    // 연필
    {
        // 연필 소환
        for (int index = 0; index < (int)count; index++) {
            Transform bullet;

            // 기존 연필 재사용
            if (index < transform.childCount) {
                bullet = transform.GetChild(index);
            }

            // 새로운 연필 생성
            else {
                bullet = StageManager.instance.poolManager.Get(prefabId).transform;
                bullet.parent = transform;
            }

            // 위치, 각도 초기화
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // 각도 설정
            Vector3 rotVec = Vector3.forward * 360 * index / (int)count;
            bullet.Rotate(rotVec);

            // 위치 설정
            bullet.Translate(bullet.up * 2f, Space.World);

            // 수치 초기화
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 is Infinity Per.
        }
    }

    void Book()     // 전공책
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        float randomAngle = Random.Range(0f, 360f);

        Transform bullet = StageManager.instance.poolManager.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.Euler(new Vector3(0, 0, randomAngle));
        bullet.GetComponent<Bullet>().Init(damage, (int)count, dir);


        // Sound
        AudioManager.instance.PlaySfx(AudioManager.SFX.Range);
    }

    void Dirty()    // 꼬질꼬질
    {
        Transform bullet = transform.GetChild(0);
        circleCollider = bullet.GetComponent<CircleCollider2D>();
        Bullet bulletDamage = bullet.GetComponent<Bullet>();

        bulletDamage.damage = damage;
        bullet.localScale = new Vector3(count, count, count);
    }
}
