using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public float speed;
    public float rotationDelta;
    public float spawnTimeDelta;
    public GameObject enemyPrefab;
    public GameObject baseObject;

    // Start is called before the first frame update
    void Start()
    {
        Spawn(50);
    }

    public void Spawn(int numberOfEnemies)
    {
        StartCoroutine(Spawner360(transform.position, numberOfEnemies)); // set this EnemySpawner in the right location before calling Spawn
    }

    IEnumerator Spawner360(Vector3 position, int numberOfEnemies)
    {
        GameObject[] enemyList = new GameObject[numberOfEnemies];

        Vector3 direction = transform.forward;
        Quaternion rotation = Quaternion.AngleAxis(rotationDelta, Vector3.up);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            newEnemy.GetComponent<EnemyController>().SetBase(baseObject);
            newEnemy.SetActive(true);
            enemyList[i] = newEnemy;
            newEnemy.GetComponent<Rigidbody>().velocity = speed * direction;
            direction = rotation * direction;

            yield return new WaitForSeconds(spawnTimeDelta);
        }
    }
}
