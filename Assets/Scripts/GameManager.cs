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

    [Header("Menus e UI")]
    public GameObject painelMainMenu;
    public GameObject hudGameplay;
    public PlayerController player; // Arraste o objeto da Nave aqui no Inspector
    
    [Header("Pause")]
    public GameObject painelPause;
    public bool jogoPausado = false;

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
        // Garante que o jogo comece no estado de Menu
        jogoAtivo = false;

        if (painelMainMenu != null) painelMainMenu.SetActive(true);
        if (hudGameplay != null) hudGameplay.SetActive(false);

        // O AsteroidSpawner continua ativo para os meteoros caírem no fundo!
        
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
            // Se a partida está rolando (ou pausada), ESC alterna o pause
            if (jogoAtivo)
            {
                AlternarPause();
            }
            // Se estiver no Main Menu (jogoAtivo == false e não pausado), ESC fecha o jogo
            else if (!jogoPausado)
            {
                SairDoJogo();
            }
        }
    }

    public void AlternarPause()
    {
        if (jogoPausado)
        {
            ResumirJogo();
        }
        else
        {
            PausarJogo();
        }
    }

    public void PausarJogo()
    {
        jogoPausado = true;
        Time.timeScale = 0f;
        if (painelPause != null) painelPause.SetActive(true);
    }

    public void ResumirJogo()
    {
        jogoPausado = false;
        Time.timeScale = 1f;
        if (painelPause != null) painelPause.SetActive(false);
    }

    // 2. Botão MAIN MENU
    public void VoltarParaMainMenu()
    {
        // Sempre restaura o tempo antes de qualquer mudança de estado
        Time.timeScale = 1f;
        jogoPausado = false;
        jogoAtivo = false;

        // Esconde o pause e o HUD de gameplay, e reabre o Main Menu
        if (painelPause != null) painelPause.SetActive(false);
        if (hudGameplay != null) hudGameplay.SetActive(false);
        if (painelMainMenu != null) painelMainMenu.SetActive(true);

        // Reseta pontuação e vidas para não vazar para a próxima partida
        pontos = 0;
        vidas = 3;
        // Atualize seus textos de UI aqui, se aplicável

        // Limpa asteroides ativos da tela (ajuste o nome da tag/componente conforme o seu projeto)
        LimparAsteroidesDaCena();

        // Reposiciona a nave fora da tela (posição inicial do menu)
        if (player != null)
        {
            player.StopAllCoroutines();
            player.transform.position = new Vector3(0f, player.posYForaDaTela, 0f);
        }
    }

    // 3. Botão EXIT
    public void SairPeloPause()
    {
        // Garante timeScale normal antes de encerrar
        Time.timeScale = 1f;
        SairDoJogo();
    }

    private void LimparAsteroidesDaCena()
    {
        // Exemplo usando Tag "Asteroid" — se usar um script próprio (ex: Asteroid), prefira FindObjectsByType<Asteroid>()
        GameObject[] asteroides = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject asteroide in asteroides)
        {
            Destroy(asteroide);
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

    // Conecte esta função no botão "Jogar" (OnClick)
    public void IniciarJogo()
    {
        if (painelMainMenu != null) painelMainMenu.SetActive(false);
        if (hudGameplay != null) hudGameplay.SetActive(true);

        // Reseta pontos e vidas (caso seja uma nova partida)
        pontos = 0;
        vidas = 3;
        // Chame aqui sua função que atualiza os textos de UI, se tiver uma

        // Chama a animação de entrada da nave pela parte de baixo
        if (player != null)
        {
            StartCoroutine(player.EntrarEmCena());
        }
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
