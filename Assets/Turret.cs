using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject asteroid;

    private void Start()
    {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            var shotAsteroid = Instantiate(asteroid, transform.position, Quaternion.identity);
            shotAsteroid.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 100f);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
