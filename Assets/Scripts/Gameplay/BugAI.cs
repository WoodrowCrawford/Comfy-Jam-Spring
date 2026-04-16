using UnityEngine;

public class BugAI : MonoBehaviour
{
    [SerializeField] float minSpeed = 2f;
    [SerializeField] float maxSpeed = 6f;
    float moveSpeed;
    Vector2 moveDirection;

    void Start()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed);
    }

    public void SetStartingDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;

        if (moveDirection.x != 0)
        {
            transform.localScale = new Vector3(moveDirection.x > 0 ? 1 : -1, 1, 1);
        }
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);

        bool isTooFar = screenPoint.x < -0.8f || screenPoint.x > 1.8f ||
                        screenPoint.y < -0.8f || screenPoint.y > 1.8f;

        if (isTooFar)
        {
            Destroy(gameObject);
        }
    }
}
