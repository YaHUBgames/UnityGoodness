using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject template;
    [SerializeField] private int spawnAmount = 1;
    [SerializeField] private int objectsPerSpawn = 1;
    [SerializeField] private float spawnObjectDelay = 0.1f;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < spawnAmount / objectsPerSpawn; i++)
        {
            for (int j = 0; j < objectsPerSpawn; j++)
                Instantiate(template, transform.position, transform.rotation, transform);

            yield return new WaitForSeconds(spawnObjectDelay);
        }
    }
}
