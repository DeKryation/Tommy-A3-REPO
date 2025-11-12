using UnityEngine;

public class KeyScript : MonoBehaviour
{
    ParticleSystem idle;
    ParticleSystem explode;
    MeshCollider meshC;
    void Start()
    {
        meshC = GetComponent<MeshCollider>();
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            idle.Stop();
            explode.Play();
            var keyUI = GameObject.Find("/Canvas/keyHUD");
            if (keyUI != null && !keyUI.activeInHierarchy)
            {
                keyUI.SetActive(true);
            }
        }
    }
}
