// Assets/Scripts/Gameplay/ParticleMovement.cs
using UnityEngine;

public class ParticleMovement : MonoBehaviour
{
    private ParticleData data;
    private NoteSpawner spawner;
    private bool isMoving = true;
    
    public void Initialize(ParticleData particleData, NoteSpawner noteSpawner)
    {
        data = particleData;
        spawner = noteSpawner;
    }
    
    void Update()
    {
        if (!isMoving) return;
        
        transform.Translate(Vector3.back * data.moveSpeed * Time.deltaTime);
        
        if (transform.position.z < -2f)
        {
            Destroy(gameObject);
        }
    }
    
    public void Capture()
    {
        if (!isMoving) return;
        
        isMoving = false;
        spawner?.OnParticleCaptured(data);
        Destroy(gameObject);
    }
}