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

    //player takes damage based on the amount passed in
    public void TakeDamage(int damageTaken)
    {
        //Debug.Log("damage taken");
        curHealth -= damageTaken;
        StartCoroutine(DamageTakenColor());
    }

    //returns whether the player is alive
    public bool isAlive()
    {
        return curHealth > 0;
    }

    //kills the player after a delay (this is used if the player goes out of bounds)
    public IEnumerator KillPlayer()
    {
        yield return new WaitForSeconds(3.5f);
        curHealth = -1;
    }

    //when the player takes damage, have them temporarily turn a different color
    IEnumerator DamageTakenColor()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(0.3f);
        sr.color = baseColor;
    }
}
