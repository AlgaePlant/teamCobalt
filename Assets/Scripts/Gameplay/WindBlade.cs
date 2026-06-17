using UnityEngine;

public class WindBlade : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 1.5f;

    private Note target;


    public void Initialize(Note note)
    {
        target = note;
    }


    void Start()
    {
        Destroy(gameObject, lifeTime);
    }


    void Update()
    {
        if (target != null)
        {
            Vector3 dir =
                (
                target.transform.position
                -
                transform.position
                )
                .normalized;


            transform.position +=
                dir * speed * Time.deltaTime;



            if (Vector3.Distance(
                transform.position,
                target.transform.position
            ) < 0.5f)
            {
                target.Capture();
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position +=
                transform.forward *
                speed *
                Time.deltaTime;
        }
    }
}