using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgBar : MonoBehaviour
{
    [SerializeField] private Image barFill;
    private float fillAmt;
    private float fillTarget;

    private static ProgBar instance;

    public static ProgBar GetInstance()
    {
        return instance;
    }

    public void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }


        // Update is called once per frame
        private void Update()
    {
        if (fillAmt < fillTarget)
        {
            fillAmt += 7 * Time.deltaTime;
            float progressNormalized = fillAmt / 100;
            barFill.fillAmount = progressNormalized;
        }
    }

    public void SetFill(float a)
    {
        fillTarget = a;
    }


}
