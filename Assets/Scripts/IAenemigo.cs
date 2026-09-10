using UnityEngine;

public class IAEnemigo : MonoBehaviour
{
    [Header("Estadísticas Base")]
    [SerializeField] private float vidaMaxima = 100f;
    private float _vidaActual;

    [Header("Rangos de Comportamiento IA")]
    [SerializeField] private float radioDeteccion = 8.0f;
    [SerializeField] private float radioAtaque = 1.8f;
    [SerializeField] private float velocidadPersecucion = 3.5f;

    [Header("Combate")]
    [SerializeField] private float dañoAtaque = 10f;
    [SerializeField] private float tiempoEntreAtaques = 1.5f;

    private Transform _playerTransform;
    private float _proximoAtaqueTime;

    private void Start()
    {
        _vidaActual = vidaMaxima;

        // Busca al jugador en la escena usando el nombre exacto de su objeto
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("⚠️ IAEnemigo: No se encontró el objeto 'Player' en la jerarquía.");
        }
    }

    private void Update()
    {
        if (_playerTransform == null || _vidaActual <= 0) return;

        float distanciaAlPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        // Máquina de estados simple: Atacar -> Perseguir -> Esperar
        if (distanciaAlPlayer <= radioAtaque)
        {
            EjecutarAtaque();
        }
        else if (distanciaAlPlayer <= radioDeteccion)
        {
            PerseguirAlPlayer();
        }
    }

    private void PerseguirAlPlayer()
    {
        Vector3 posicionObjetivo = new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z);
        transform.LookAt(posicionObjetivo);

        transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidadPersecucion * Time.deltaTime);
    }

    private void EjecutarAtaque()
    {
        if (Time.time >= _proximoAtaqueTime)
        {
            Debug.Log($"⚔️ IA Enemigo: Lanzó un ataque e infligió {dañoAtaque} de daño a Yin Lao.");

            // Aquí se conectará con el daño real al Player en el siguiente paso
            // _playerTransform.GetComponent<PlayerHealth>().RecibirDaño(dañoAtaque);

            _proximoAtaqueTime = Time.time + tiempoEntreAtaques;
        }
    }

    // Este es el método público que llamará el script 'PlayerCombat' al golpearlo
    public void AplicarDaño(float cantidad)
    {
        if (_vidaActual <= 0) return;

        _vidaActual -= cantidad;
        Debug.Log($"💥 IA Enemigo: ¡Recibió impacto! Vida restante: {_vidaActual}/{vidaMaxima}");

        if (_vidaActual <= 0)
        {
            ProcesarMuerte();
        }
    }

    private void ProcesarMuerte()
    {
        Debug.Log("💀 IA Enemigo: Derrotado con éxito.");
        Destroy(gameObject, 0.2f); // Destruye el cubo de pruebas
    }

    // Dibuja los rangos de la IA en el editor de Unity para calibrar distancias visualmente
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion); // Círculo de alerta
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAtaque);    // Círculo de rango de golpe
    }
}
