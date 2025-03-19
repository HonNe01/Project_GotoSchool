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

                    break;  // ����
                case 1:
                    transform.Rotate(Vector3.forward * speed * 0.5f * Time.deltaTime);

                    break;  // ��������
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
                    break;  // ����å
                default:

                    break;
            }
        }
    }

    public void LevelUp(float damage, float count)
    {
        // ���� �ɷ�ġ ����
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

    public void Init(ItemData data)     // ���� ����
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
                case 0:     // ����
                    speed = 100 * Character.WeaponSpeed;
                    Pencil();

                    break;
                case 1:     // ��������
                    Transform bullet = StageManager.instance.poolManager.Get(prefabId).transform;
                    bullet.parent = transform;

                    // ��ġ, ���� �ʱ�ȭ
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
                case 0:     // ����å
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

    void InHand(ItemData data)          // ���� ����
    {
        Hand hand = null;

        // ���� Ÿ�԰� itemId�� ���� �ش� ���� ����
        if (data.itemType == ItemData.ItemType.Melee)
        {
            hand = player.hands[(int)Hand.HandType.Left];
        }
        else if (data.itemType == ItemData.ItemType.Range)
        {
            // Range, 0 (����å)�� Back, �������� Right
            hand = (data.itemId == 0) ? player.hands[(int)Hand.HandType.Back] : player.hands[(int)Hand.HandType.Right];
        }

        // �ش� ������ ����ִ� ��츸 ����
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

    void Pencil()    // ����
    {
        // ���� ��ȯ
        for (int index = 0; index < (int)count; index++) {
            Transform bullet;

            // ���� ���� ����
            if (index < transform.childCount) {
                bullet = transform.GetChild(index);
            }

            // ���ο� ���� ����
            else {
                bullet = StageManager.instance.poolManager.Get(prefabId).transform;
                bullet.parent = transform;
            }

            // ��ġ, ���� �ʱ�ȭ
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // ���� ����
            Vector3 rotVec = Vector3.forward * 360 * index / (int)count;
            bullet.Rotate(rotVec);

            // ��ġ ����
            bullet.Translate(bullet.up * 2f, Space.World);

            // ��ġ �ʱ�ȭ
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 is Infinity Per.
        }
    }

    void Book()     // ����å
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

    void Dirty()    // ��������
    {
        Transform bullet = transform.GetChild(0);
        circleCollider = bullet.GetComponent<CircleCollider2D>();
        Bullet bulletDamage = bullet.GetComponent<Bullet>();

        bulletDamage.damage = damage;
        bullet.localScale = new Vector3(count, count, count);
    }
}
