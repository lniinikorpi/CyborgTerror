using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Image healthPoint;
    public Transform healthParent;
    List<Image> spawnedHealths = new List<Image>();
    public Player player;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeHUD(player.maxHealth);
    }

    public void InitializeHUD(int hitPoints)
    {
        hitPoints /= 4;
        for (int i = 0; i < hitPoints; i++)
        {
            Image rI= Instantiate(healthPoint, healthParent);
            rI.fillAmount = 0;
            spawnedHealths.Add(rI);
        }

        StartCoroutine(UpdateHealthHUD(12, 0));
    }

    public IEnumerator UpdateHealthHUD(int currentHealth, int startHealth)
    {
        int hearthIndex = startHealth / 4;
        if(startHealth % 4 == 0 && startHealth > 0)
        {
            hearthIndex--;
        }
        float targetFill;
        if (currentHealth < startHealth)
        {
            targetFill = spawnedHealths[hearthIndex].fillAmount - .25f;
            for (int i = startHealth; i > currentHealth; i--)
            {
                while (spawnedHealths[hearthIndex].fillAmount > targetFill)
                {
                    float step = 5 * Time.deltaTime;
                    if(spawnedHealths[hearthIndex].fillAmount - step >= targetFill)
                    {
                        spawnedHealths[hearthIndex].fillAmount -= step;
                    }
                    else
                    {
                        spawnedHealths[hearthIndex].fillAmount = targetFill;
                    }
                    yield return new WaitForSeconds(0.002f);
                }
                targetFill -= .25f;
                if (targetFill < 0)
                    targetFill = 0;
                if (spawnedHealths[hearthIndex].fillAmount == 0)
                {
                    hearthIndex--;
                }
            }
        }
        else
        {
            targetFill = spawnedHealths[hearthIndex].fillAmount + .25f;
            for (int i = startHealth; i < currentHealth; i++)
            {
                while (spawnedHealths[hearthIndex].fillAmount < targetFill)
                {
                    float step = 5 * Time.deltaTime;
                    if (spawnedHealths[hearthIndex].fillAmount + step <= targetFill)
                    {
                        spawnedHealths[hearthIndex].fillAmount += step;
                    }
                    else
                    {
                        spawnedHealths[hearthIndex].fillAmount = targetFill;
                    }
                    yield return new WaitForSeconds(0.002f);
                }
                targetFill += .25f;
                if (targetFill > 1)
                    targetFill = 1;
                if (spawnedHealths[hearthIndex].fillAmount == 1 && hearthIndex < spawnedHealths.Count - 1)
                {
                    hearthIndex++;
                }
            }
        }
    }

}
