using UnityEngine;

public class Result : MonoBehaviour
{
    public GameObject[] titles;

    public void Lose()
    {
        titles[1].SetActive(false);
        titles[0].SetActive(true);
    }

    public void Win()
    {
        titles[0].SetActive(false);
        titles[1].SetActive(true);
    }
}
