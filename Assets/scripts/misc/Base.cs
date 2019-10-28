using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    protected float maxHealth = 1;
    protected float currentHealth = 1;

    public void takeDamge(float dammage)
    {
        currentHealth -= dammage;
        if (currentHealth < maxHealth)
        {
            DestroyThis();
        }
    }
    protected virtual void DestroyThis()
    {
        Destroy(gameObject);
    }
}
