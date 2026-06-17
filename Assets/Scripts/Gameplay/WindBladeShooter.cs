using UnityEngine;

public class WindBladeShooter : MonoBehaviour
{
    public GameObject yellowBladePrefab;
    public GameObject greenBladePrefab;
    public GameObject bluePurpleBladePrefab;

    public Transform spawnPoint;

    private void OnEnable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic += Shoot;
    }

    private void OnDisable()
    {
        KeyboardGuitarSimulator.OnStringPlayedStatic -= Shoot;
    }

    void Shoot(int id)
    {
        switch (id)
        {
            case 1:
                Fire(yellowBladePrefab, NoteColor.Yellow);
                break;

            case 2:
                Fire(greenBladePrefab, NoteColor.Green);
                break;

            case 3:
                Fire(bluePurpleBladePrefab, NoteColor.BluePurple);
                break;
        }
    }

    void Fire(GameObject prefab, NoteColor color)
    {
        if (prefab == null)
            return;

        Note target = HitManager.FindTarget(color);

        GameObject blade = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        WindBlade wb = blade.GetComponent<WindBlade>();

        if (wb != null)
        {
            wb.Initialize(target);
        }
    }
}