using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    public GameObject prefabColetavel;
    public float intervaloSpawn = 4f;
    public float limiteXSpawn = 8f;

    private float timer;
    private bool podeGerarColetaveis = true;

    void Update()
    {
        if (!podeGerarColetaveis) return;
        timer += Time.deltaTime;

        if (timer >= intervaloSpawn)
        {
            GerarColetavel();
            timer = 0f;
        }
    }

    void GerarColetavel()
    {
        if (prefabColetavel == null) return;

        float posXAleatoria = Random.Range(-limiteXSpawn, limiteXSpawn);
        Vector3 posicaoSpawn = new Vector3(posXAleatoria, 6f, 0f);

        Instantiate(prefabColetavel, posicaoSpawn, Quaternion.identity);
    }

    public void PararGeracaoColetaveis()
    {
        podeGerarColetaveis = false;
    }
}
