using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 8f;

    [Header("Limites da Tela")]
    public float limiteX = 8.3f;
    public float limiteYSuperior = 4.5f;
    public float limiteYInferior = -4.5f;

    [Header("Estados do Jogo")]
    public bool podeMover = true;

    private Rigidbody2D rb;
    private float movimentoHorizontal;
    private float movimentoVertical;

    public SpriteRenderer renderizadorNave;
    public Sprite spriteDano;
    public Sprite spriteDano1;
    public Sprite spriteDano2;
    [Header("Animação de Entrada")]
    public float posYForaDaTela = -7f; // Posição escondida lá embaixo
    public float posYInicial = -3.5f;  // Posição onde a nave para para jogar
    public float velocidadeEntrada = 4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Já deixa a nave escondida lá embaixo antes de apertar Jogar
        transform.position = new Vector3(0f, posYForaDaTela, 0f);
    }

    void Update()
    {
        if (!podeMover) return;

        movimentoHorizontal = 0f;
        movimentoVertical = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                movimentoHorizontal = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                movimentoHorizontal = 1f;

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                movimentoVertical = -1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                movimentoVertical = 1f;
        }
    }

    void FixedUpdate()
    {
        if (!podeMover) return;

        Vector2 novaVelocidade = new Vector2(movimentoHorizontal * velocidade, movimentoVertical * velocidade);

        float posX = transform.position.x;
        if ((posX >= limiteX && movimentoHorizontal > 0) || (posX <= -limiteX && movimentoHorizontal < 0))
        {
            novaVelocidade.x = 0;
        }

        float posY = transform.position.y;
        if ((posY >= limiteYSuperior && movimentoVertical > 0) || (posY <= limiteYInferior && movimentoVertical < 0))
        {
            novaVelocidade.y = 0;
        }

        rb.linearVelocity = novaVelocidade;
    }

    public IEnumerator EntrarEmCena()
    {
        // Garante que ela comece no centro inferior, fora da tela
        transform.position = new Vector3(0f, posYForaDaTela, 0f);

        // Sobe suavemente até atingir a posição inicial de gameplay
        while (transform.position.y < posYInicial)
        {
            transform.position += Vector3.up * velocidadeEntrada * Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // Crava na posição exata
        transform.position = new Vector3(0f, posYInicial, 0f);

        // Libera o jogo e os controles!
        GameManager.Instance.jogoAtivo = true;
    }

    public void AtualizarAparenciaNave(int vidasRestantes)
    {
        if (renderizadorNave == null) return;

        if (vidasRestantes == 2)
        {
            renderizadorNave.sprite = spriteDano1;
        }
        else if (vidasRestantes == 1)
        {
            renderizadorNave.sprite = spriteDano2;
        }
        else if (vidasRestantes <= 0)
        {
            renderizadorNave.sprite = spriteDano;
        }
    }

    public void PararMovimento()
    {
        podeMover = false;
        rb.linearVelocity = Vector2.zero;
    }
}
