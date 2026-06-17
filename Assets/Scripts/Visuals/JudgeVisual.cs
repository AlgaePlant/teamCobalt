using UnityEngine;
using System.Collections;

public class JudgeVisual : MonoBehaviour
{
    public Renderer[] parts;

    public Color idleColor = new Color(1, 1, 1, 0.25f);

    public Color hitColor = Color.white;

    public float flashTime = 0.12f;

    void Start()
    {
        SetColor(idleColor);
    }

    public void PlayHit()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        SetColor(hitColor);

        yield return new WaitForSeconds(flashTime);

        SetColor(idleColor);
    }

    void SetColor(Color c)
    {
        foreach (Renderer r in parts)
        {
            if (r == null)
                continue;

            r.material.color = c;
        }
    }
}