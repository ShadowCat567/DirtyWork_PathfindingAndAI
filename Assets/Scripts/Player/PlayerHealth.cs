using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    //these will be the most useful for setting up health UI //It is, thank you Leena - Kimari
    int curHealth;
    int maxHealth = 200; //originally 20

    //Additional Health UI stuff
    public Slider Slide;


    Color baseColor;
    Color damageColor = Color.magenta;

    SpriteRenderer sr;

    [SerializeField] GameObject playerDied;

    //if you set up the logic for the healthbar in another script, use these to get player health values
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

        //Kimari Doing prototype healthbar stuff
        Slide.minValue = 0;
        Slide.maxValue = maxHealth;

        Slide.value = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (curHealth <= 0)
        {
            this.gameObject.GetComponent<PlayerMovement>().enabled = false;
            playerDied.SetActive(true);
            //Debug.Log("player is dead");
        }
    }

    public void TakeDamage(int damageTaken)
    {
        //Debug.Log("damage taken");
        curHealth -= damageTaken;
        StartCoroutine(DamageTakenColor());

        //Kimari Heath UI stuff
        Slide.value = curHealth;
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
