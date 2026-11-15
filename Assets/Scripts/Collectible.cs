using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Configurações do Coletável")]
    public int pontosConcedidos = 10;
    public float velocidade = 4f;

    [Header("Configuração Visual")]
    private float velocidadeRotacao;

    [Header("SFX")]
    [SerializeField] private AudioSource coletavelSFX;

    void Start()
    {
        velocidadeRotacao = Random.Range(-90f, 90f);
    }

    void Update()
    {
        // Move o coletável para baixo continuamente
        transform.position += Vector3.down * velocidade * Time.deltaTime;

        // Gira o coletável enquanto cai
        transform.Rotate(Vector3.forward * velocidadeRotacao * Time.deltaTime);

        // Se passar da tela, é destruído
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o player encostou no coletável
        if (collision.CompareTag("Player"))
        {
            // Adiciona pontos através do GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AdicionarPontos(pontosConcedidos);
                if (coletavelSFX != null)
                {
                    coletavelSFX.Play();
                }
            }

            // Destrói o coletável após ser pego
            Destroy(gameObject);
        }
    }
}
