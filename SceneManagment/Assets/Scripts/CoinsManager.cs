using UnityEngine;

public class CoinsManager : MonoBehaviour
{
    public GameObject goal; 
    int collected = 0;
    public int total = 3;

    public void AddCoin()
    {
        collected++;
        if (collected >= total)
            goal.SetActive(true);
    }
}
