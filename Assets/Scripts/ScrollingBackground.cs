using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Header("Configurações do Fundo")]
    public float velocidade = 2f;
    public float alturaFundo = 18.43f;

    private Transform fundo1;
    private Transform fundo2;

    void Start()
    {
        if (transform.childCount >= 2)
        {
            fundo1 = transform.GetChild(0);
            fundo2 = transform.GetChild(1);
        }
        else
        {
            Debug.LogError("O objeto Pai precisa ter pelo menos 2 imagens filhas");
        }
    }

    void Update()
    {
        if (fundo1 == null || fundo2 == null) return;

        fundo1.position += Vector3.down * velocidade * Time.deltaTime;
        fundo2.position += Vector3.down * velocidade * Time.deltaTime;

        if (fundo1.position.y <= -alturaFundo)
        {
            fundo1.position = new Vector3(fundo1.position.x, fundo2.position.y + alturaFundo, fundo1.position.z);
        }

        if (fundo2.position.y <= -alturaFundo)
        {
            fundo2.position = new Vector3(fundo2.position.x, fundo1.position.y + alturaFundo, fundo2.position.z);
        }
    }
}
