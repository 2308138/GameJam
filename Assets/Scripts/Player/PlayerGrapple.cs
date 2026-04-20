using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxDistance = 0F;
    public float pullSpeed = 0F;
    public LayerMask grappleLayer;
    private bool isGrappling = false;
    private Vector2 targetPoint;

    // Private References
    private LineRenderer lr;
    private Rigidbody2D rb;
    private PlayerController controls;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerController();

        controls.Player.Grapple.performed += _ => StartGrapple();
        controls.Player.Grapple.canceled += _ => StopGrapple();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void StartGrapple()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            targetPoint = hit.point;
            isGrappling = true;
            lr.enabled = true;

            GetComponent<PlayerMovement>().ToggleMovement(false);
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;
        lr.enabled = false;
        rb.linearVelocity = Vector2.zero;
        GetComponent<PlayerMovement>().ToggleMovement(true);
    }

    private void Update()
    {
        if (isGrappling)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, targetPoint);

            transform.position = Vector2.MoveTowards(transform.position, targetPoint, pullSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPoint) > 0.2F)
            {
                StopGrapple();
            }
        }
    }
}