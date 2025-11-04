using UnityEngine;

public class PickupOnTouch : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
{
    if (!other.CompareTag("Player")) return;
    var c = GetComponent<Collectible>();
    if (GameDirector.I != null) GameDirector.I.HandleCollect(c);
    Destroy(gameObject);
}

}
