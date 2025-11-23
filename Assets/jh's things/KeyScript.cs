using System.Collections;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public ParticleSystem explode;
    public GameObject keyHud;
    public GameObject self;
    void Start()
    {
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            explode.Play();
            if (keyHud != null && !keyHud.activeInHierarchy)
            {
                keyHud.SetActive(true);
            }
            ItemManager.GetInstance().TryGettingItem(GetComponent<ItemData>());
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.3f);
        self.SetActive(false);
    }

}
