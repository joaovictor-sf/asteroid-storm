using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Configurações do Asteroide")]
    public float velocidadeMinima = 3f;
    public float velocidadeMaxima = 6f;
    private float velocidadeAtual;

    [Header("Configurações Visuais")]
    public float tamanhoMinimo = 2.6f;
    public float tamanhoMaximo = 6f;

    private float velocidadeRotacao;
    private bool estaMovendo = true;

    // Recebe e define as configurações enviadas pelo Spawner ao nascer
    public void ConfigurarDificuldade(float vMin, float vMax, float tMin, float tMax)
    {
        velocidadeMinima = vMin;
        velocidadeMaxima = vMax;
        tamanhoMinimo = tMin;
        tamanhoMaximo = tMax;
    }

    void Start()
    {
        velocidadeAtual = Random.Range(velocidadeMinima, velocidadeMaxima);

        float tamanhoAleatorio = Random.Range(tamanhoMinimo, tamanhoMaximo);
        transform.localScale = new Vector3(tamanhoAleatorio, tamanhoAleatorio, 1f);

        velocidadeRotacao = Random.Range(-120f, 120f);
    }

    void Update()
    {
        if (!estaMovendo) return;

        transform.position += Vector3.down * velocidadeAtual * Time.deltaTime;
        transform.Rotate(Vector3.forward * velocidadeRotacao * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            DestruirAsteroide();
        }
    }

    public void PararMovimento()
    {
        estaMovendo = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("IsDestroyed");
        }
    }

    void DestruirAsteroide()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PerderVida();
            }

            DestruirAsteroide();
        }
    }
}
