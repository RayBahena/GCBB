using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {
    public int health;
    public int numOfcroissants;
    public Image[] croisants;
    public Sprite fullCroissant;
    public Sprite emptyCroissant;
    public GameManager gameManager;
     void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
    }
    public void Update()
    {
        health = gameManager.NumberOfLives;
        if (health > numOfcroissants)
        {
            health = numOfcroissants;
        }
        
        for (int i = 0; i < croisants.Length; i++)
        {
            if (i < health)
            {
                croisants[i].sprite = fullCroissant;
            }
            else
            {
                croisants[i].sprite = emptyCroissant;
            }
            if (i < numOfcroissants)
            {
                croisants[i].enabled = true;
            }
            else
            {
                croisants[i].enabled = false;
            }
        }
    }

}
