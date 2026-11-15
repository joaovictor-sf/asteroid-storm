using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Variáveis de Jogo")]
    public int pontos = 0;
    public int vidas = 3;
    public bool jogoAtivo = true;

    [Header("Configurações de Dificuldade")]
    public int pontosParaAumentarDificuldade = 50;
    private int proximoNivelDificuldade = 50;

    [Header("Referências de UI")]
    public TextMeshProUGUI textoPontos;
    public TextMeshProUGUI textoVidas;
    public GameObject painelGameOver;

    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AtualizarUI();
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
    }

    void Update()
    {
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SairDoJogo();
        }
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    
    public void AdicionarPontos(int quantidade)
    {
        if (!jogoAtivo) return;

        pontos += quantidade;
        AtualizarUI();

        if (pontos >= proximoNivelDificuldade)
        {
            AumentarDificuldade();
            proximoNivelDificuldade += pontosParaAumentarDificuldade; 
        }
    }

    void AumentarDificuldade()
    {
      
        AsteroidSpawner spawner = FindAnyObjectByType<AsteroidSpawner>();
        if (spawner != null)
        {
            spawner.AumentarVelocidadeAsteroides(1.1f); // Aumenta a velocidade em 10%
        }
    }

    
    public void PerderVida()
    {
        if (!jogoAtivo) return;

        vidas--;
        AtualizarUI();

        
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.AtualizarAparenciaNave(vidas);
        }

        if (vidas <= 0)
        {
            GameOver();
        }
    }

    
    void AtualizarUI()
    {
        if (textoPontos != null)
        {
            textoPontos.text = "Pontos: " + pontos;
        }
        if (textoVidas != null)
        {
            textoVidas.text = "Vidas: " + vidas;
        }
    }

    void GameOver()
    {
        jogoAtivo = false;

        
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.PararMovimento();
        }

        AsteroidSpawner spawner = FindAnyObjectByType<AsteroidSpawner>();
        if (spawner != null)
        {
            spawner.PararGeracaoAsteroides();
        }

        CollectibleSpawner collectibleSpawner = FindAnyObjectByType<CollectibleSpawner>();
        if (collectibleSpawner != null)
        {
            collectibleSpawner.PararGeracaoColetaveis();
        }

        
        StartCoroutine(LimparAsteroidesGradualmente());
    }

    
    IEnumerator LimparAsteroidesGradualmente()
    { 
        
        GameObject[] todosAsteroidesObj = GameObject.FindGameObjectsWithTag("Enemy");

        yield return new WaitForSeconds(0.2f);

        float tempoTotalEfeito = 1.0f;
        float intervaloCalculado = todosAsteroidesObj.Length > 0 ? tempoTotalEfeito / todosAsteroidesObj.Length : 0.1f;
        float intervalo = Mathf.Clamp(intervaloCalculado, 0.05f, 0.2f);

        for (int i = 0; i < todosAsteroidesObj.Length; i++)
        {
            if (todosAsteroidesObj[i] != null)
            {
                Asteroid ast = todosAsteroidesObj[i].GetComponent<Asteroid>();
                if (ast != null)
                {
                    ast.PararMovimento();
                }

                Destroy(todosAsteroidesObj[i], 0.5f);

                yield return new WaitForSeconds(intervalo);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
        }
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
