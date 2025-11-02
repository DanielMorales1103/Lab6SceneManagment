using UnityEngine;

public class ZoneGoal : MonoBehaviour
{
    public GameObject goal; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Cube") || other.CompareTag("Cube")) 
        {
            goal.SetActive(true);
        }
    }
}
