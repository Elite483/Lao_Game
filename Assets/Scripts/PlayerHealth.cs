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

        MovimientoRPG scriptMovimiento = GetComponent<MovimientoRPG>(); 
        if(scriptMovimiento != null)
        {
            scriptMovimiento.enabled = false;
        }

        PlayerCombat scriptCombate = GetComponent<PlayerCombat>();
        if(scriptCombate != null)
        {
            scriptCombate.enabled = false;
        }

        CharacterController controller = GetComponent<CharacterController>();
        if(controller != null)
        {
            controller.enabled = false;
        }

        MeshRenderer mallaCapsula = GetComponent<MeshRenderer>();
        if(mallaCapsula != null)
        {
            mallaCapsula.enabled = false; 
        }
    }
}
