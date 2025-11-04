using UnityEngine;

public enum CatchColor { Red, Green, Blue }

public class Collectible : MonoBehaviour
{
    [SerializeField] CatchColor myColor = CatchColor.Red;
    public CatchColor MyColor => myColor;

    [Header("Hover & Spin")]
    [SerializeField] float spinSpeed = 90f;
    [SerializeField] float bobHeight = 0.15f;
    [SerializeField] float bobSpeed = 2f;
    Vector3 basePos;

    void Start() => basePos = transform.position;

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = basePos + new Vector3(0f, Mathf.Sin(Time.time * bobSpeed) * bobHeight, 0f);
    }
}
