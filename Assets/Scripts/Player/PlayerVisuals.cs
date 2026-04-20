using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    // Private References
    private Rigidbody2D rb;
    private Vector3 originalScale;

    private void Start()
    {
        rb= GetComponentInParent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float stretch = Mathf.Abs(rb.linearVelocity.y) * 0.01F;
        transform.localScale = new Vector3(originalScale.x - stretch, originalScale.y + stretch, originalScale.z);

        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 10F);
    }
}