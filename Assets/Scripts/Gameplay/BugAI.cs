using UnityEngine;

public class BugAI : MonoBehaviour
{
    [SerializeField] float minSpeed = 2f;
    [SerializeField] float maxSpeed = 6f;
    float moveSpeed;
    Vector2 moveDirection;
    float screenBoundX;
    float screenBoundY;

    void Start()
    {
        screenBoundX = Camera.main.orthographicSize * Camera.main.aspect;
        screenBoundY = Camera.main.orthographicSize;

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

        float destructionMargin = BugManager.spawnBuffer + 10f;

        if (Mathf.Abs(transform.position.x) > screenBoundX + destructionMargin ||
            Mathf.Abs(transform.position.y) > screenBoundY + destructionMargin)
        {
            Destroy(gameObject);
        }
    }
}
