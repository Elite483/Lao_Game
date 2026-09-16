using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       health = 100; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveDamage(float damage)
    {

        health -= damage;

        if(health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //El jugador murio
        Debug.Log("EL JUGADOR MURIO");
    }
}
