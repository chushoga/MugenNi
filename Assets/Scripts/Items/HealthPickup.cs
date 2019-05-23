using UnityEngine;

public class HealthPickup : MonoBehaviour {

    public GameObject particle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            other.GetComponent<PlayerController>().AddHealth(1);
            Instantiate(particle, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
