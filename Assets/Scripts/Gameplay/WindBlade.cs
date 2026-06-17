using UnityEngine;

public class WindBlade : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 1.5f;

    private Vector3 direction;
    private Note target;

    public void Initialize(Note note)
    {
        target = note;

        if (target != null)
        {
            direction = (target.transform.position - transform.position).normalized;
        }
        else
        {
            direction = transform.forward;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (target == null)
            return;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.7f)
        {
            target.Capture();
            Destroy(gameObject);
        }
    }
}