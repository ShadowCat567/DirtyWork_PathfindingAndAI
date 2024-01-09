using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    int curHealth;
    int maxHealth = 200; 

    Color baseColor;
    Color damageColor = Color.magenta;

    SpriteRenderer sr;

    [SerializeField] GameObject playerDied;

    //gets the max player health
    public int getPlayerMaxHealth() { return maxHealth; }
    //gets the current player health
    public int getPlayerCurrentHealth() { return curHealth; }

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        curHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (curHealth <= 0)
        {
            //Debug.Log("player is dead");
        }
    }

    public void TakeDamage(int damageTaken)
    {
        //Debug.Log("damage taken");
        curHealth -= damageTaken;
        StartCoroutine(DamageTakenColor());
    }

    public bool isAlive()
    {
        return curHealth > 0;
    }

    public IEnumerator KillPlayer()
    {
        yield return new WaitForSeconds(3.5f);
        curHealth = -1;
    }

    IEnumerator DamageTakenColor()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(0.3f);
        sr.color = baseColor;
    }
}
