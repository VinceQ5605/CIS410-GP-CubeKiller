using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Range(.1f,20f)]
    public float explosionRadius;
    [Range(0f, 30f)]
    public float explosionIntensity;
    //public int damage;
    GameObject grenade;

    public void SetValues(float radius, float intensity, GameObject grenade)
    {
        explosionRadius = radius;
        explosionIntensity = intensity;
        this.grenade = grenade;
    }

    public void Explode()
    {
        StartCoroutine(ExplosionVisual());

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject gameObject = hitCollider.gameObject;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            Vector3 direction = gameObject.transform.position - transform.position;

            direction.Normalize();
            EnemyController enemyController = gameObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                gameObject.GetComponent<EnemyController>().Hit(explosionIntensity * direction);
            }
        }
    }

    private IEnumerator ExplosionVisual()
    {
        Transform transform = grenade.transform;
        Vector3 position = transform.position;
        grenade.GetComponent<Rigidbody>().isKinematic = false;
        grenade.GetComponent<Rigidbody>().useGravity = false;
        grenade.GetComponent<SphereCollider>().isTrigger = true;
        
        transform.localScale = new Vector3(.1f, .1f, .1f);

        for (int i = 1; i < explosionIntensity + 1; i += 4)
        {
            transform.localScale = i*(new Vector3(.1f, .1f, .1f));
            transform.position = position;
            yield return null;
        }
        grenade.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
