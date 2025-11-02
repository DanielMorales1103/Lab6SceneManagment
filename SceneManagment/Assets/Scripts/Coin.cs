using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    private float rotationSpeed = 180f;   
    private float bobAmplitude = 0.25f;  
    private float bobFrequency = 1.2f;    

    private string playerTag = "Player";

    Vector3 basePos;
    float phase; 

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        basePos = transform.position;
        phase = Random.value * Mathf.PI * 2f; 
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f, Space.Self);

        float y = basePos.y + bobAmplitude * Mathf.Sin(2f * Mathf.PI * bobFrequency * Time.time + phase);
        var p = transform.position;
        p.y = y;
        transform.position = p;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            var mgr = Object.FindFirstObjectByType<CoinsManager>();
            if (mgr) mgr.AddCoin();
            Destroy(gameObject);
        }
    }
}
