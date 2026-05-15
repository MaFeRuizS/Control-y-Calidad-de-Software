using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class DatosControlMatematicas
{
    public int joyX = 2048;
    public int joyY = 2048;
    public int joyBtn;
    public int A;
    public int B;
    public int X;
    public int Y;
}

public class InputArcadeManager : MonoBehaviour
{
    public static InputArcadeManager Instance;

    [Header("Configuración Global")]
    public string ipESP32 = "192.168.1.71";
    public DatosControlMatematicas estadoActual = new DatosControlMatematicas();

    void Awake()
    {
        // PATRÓN SINGLETON: Si ya existe uno, se destruye el nuevo. Si no, este sobrevive.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(LeerESP32Continua());
    }

    IEnumerator LeerESP32Continua()
    {
        string url = "http://" + ipESP32.Trim() + "/estado";
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    estadoActual = JsonUtility.FromJson<DatosControlMatematicas>(webRequest.downloadHandler.text);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}