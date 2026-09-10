using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuración del Combo")]
    [SerializeField] private int maxComboSteps = 3;
    [SerializeField] private float comboResetTime = 1.0f;

    private int _currentComboStep = 0;
    private float _lastAttackTime = 0f;
    private bool _isAttacking = false;

    [Header("Ataque por Distancia Directa")]
    [SerializeField] private float attackRange = 2.5f; // Subido a 2.5 por seguridad
    [SerializeField] private float baseDamage = 20f;

    private void Update()
    {
        // SISTEMA TRADICIONAL: Detecta el click izquierdo del mouse directo (0 = Click Izquierdo)
        if (Input.GetMouseButtonDown(0) && !_isAttacking)
        {
            ProcesarGolpeCombo();
        }

        if (_currentComboStep > 0 && Time.time - _lastAttackTime > comboResetTime && !_isAttacking)
        {
            ResetCombo();
        }
    }

    private void ProcesarGolpeCombo()
    {
        _isAttacking = true;
        _currentComboStep++;

        if (_currentComboStep > maxComboSteps)
        {
            _currentComboStep = 1;
        }

        _lastAttackTime = Time.time;

        Debug.Log($"⚔️ ¡ZAS! Yin Lao ejecuta el GOLPE: #_{_currentComboStep}_");

        // Busca al enemigo directo por su script en la escena
        IAEnemigo enemigoEnEscena = Object.FindFirstObjectByType<IAEnemigo>();

        if (enemigoEnEscena != null)
        {
            float distancia = Vector3.Distance(transform.position, enemigoEnEscena.transform.position);

            if (distancia <= attackRange)
            {
                float dañoFinal = baseDamage * (1f + (_currentComboStep * 0.2f));
                enemigoEnEscena.AplicarDaño(dañoFinal);
            }
            else
            {
                Debug.Log("💨 El golpe falló: El enemigo está demasiado lejos.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró ningún objeto con el script 'IAEnemigo' en la escena.");
        }

        StartCoroutine(FinDeAnimacionAtaque(0.3f));
    }

    private IEnumerator FinDeAnimacionAtaque(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        _isAttacking = false;
    }

    private void ResetCombo()
    {
        _currentComboStep = 0;
        Debug.Log("🔄 El combo se ha reiniciado.");
    }
}