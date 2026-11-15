using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject prefabAsteroide;
    public float intervaloSpawn = 1.5f;
    public float limiteXSpawn = 8f;
    private bool podeGerarAsteroides = true;

    [Header("Limites Atuais de Dificuldade (Modificados com a Pontuação)")]
    public float velocidadeMinimaGlobal = 3f;
    public float velocidadeMaximaGlobal = 6f;
    public float tamanhoMinimoGlobal = 2.6f;
    public float tamanhoMaximoGlobal = 6f;

    private float timer;
    private bool alternarFrequencia = false;

    void Update()
    {
        if (!podeGerarAsteroides) return;

        timer += Time.deltaTime;

        if (timer >= intervaloSpawn)
        {
            GerarAsteroide();
            timer = 0f;
        }
    }

    void GerarAsteroide()
    {
        if (prefabAsteroide == null) return;

        float posicaoXAleatoria = Random.Range(-limiteXSpawn, limiteXSpawn);
        Vector3 posicaoSpawn = new Vector3(posicaoXAleatoria, 6f, 0f);

        GameObject novoAsteroide = Instantiate(prefabAsteroide, posicaoSpawn, Quaternion.identity);

        Asteroid ast = novoAsteroide.GetComponent<Asteroid>();
        if (ast != null)
        {
            ast.ConfigurarDificuldade(velocidadeMinimaGlobal, velocidadeMaximaGlobal, tamanhoMinimoGlobal, tamanhoMaximoGlobal);
        }
    }

    public void AumentarVelocidadeAsteroides(float multiplicador)
    {
        tamanhoMinimoGlobal *= multiplicador;
        tamanhoMaximoGlobal *= multiplicador;

        if (!alternarFrequencia)
        {
            velocidadeMinimaGlobal *= multiplicador;
            velocidadeMaximaGlobal *= multiplicador;
            intervaloSpawn /= multiplicador;
            alternarFrequencia = true;
        }
        else
        {
            alternarFrequencia = false;
        }
    }

    public void PararGeracaoAsteroides()
    {
        podeGerarAsteroides = false;
    }
    
}
