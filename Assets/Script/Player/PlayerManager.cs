using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header(" # Player Info")]
    public float baseMaxHealth = 100f;
    public float bonusHealth = 0f;
    public float health;
    public float MaxHealth => baseMaxHealth * (1 + bonusHealth);
    public float buffMultiplier = 0f;

    public int level;
    public int kill;
    public int exp;
    public int[] maxExp = { 3, 5, 10, 20, 25, 30, 40, 50, 60, 80, 100 };

    [Header("# Player Object")]
    public Player player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void GetExp()
    {
        if (exp >= maxExp[maxExp.Length - 1])
            return;

        exp++;

        if (exp >= maxExp[level - 1])
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        exp = 0;
        StageManager.instance.uiLevelUp.Show();
        Debug.Log("Player leveled up to: " + level);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            StageManager.instance.GameOver();
        }
    }

    public void Heal()
    {
        health = MaxHealth;
    }

    public void ResetPlayer()
    {
        health = MaxHealth;
        exp = 0;
        level = 1;
        kill = 0;

        Debug.Log("Player Set Complete");
    }
}
