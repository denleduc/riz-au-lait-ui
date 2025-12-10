using System.Collections.Generic;
using UnityEngine;
using Unity.InferenceEngine;
using System.Text;
using Unity.Collections;
using Newtonsoft.Json;
using Unity.AppUI.UI;
using TMPro;
using System.Threading.Tasks;


/*
 NOTE : Utiliser ce modèle reste le meilleur choix pour le Quest mais expose à une perte de FPS d'environ 50 (72 FPS -> 20 FPS) pendant l'inférence (entre 1 et 5 secondes).
 */
public class RunWhisper : MonoBehaviour
{
    Worker decoder1, decoder2, encoder, spectrogram;
    Worker argmax;

    private AudioClip audioClip;
    private string microphone_name;
    private float duree_defaut = 30f;
    public event System.Action<string> OnTranscriptionComplete;

    // This is how many tokens you want. It can be adjusted.
    const int maxTokens = 100;

    // Special tokens see added tokens file for details
    const int END_OF_TEXT = 50257;
    const int START_OF_TRANSCRIPT = 50258;
    const int ENGLISH = 50259;
    const int GERMAN = 50261;
    const int FRENCH = 50265;
    const int TRANSCRIBE = 50359; //for speech-to-text in specified language
    const int TRANSLATE = 50358;  //for speech-to-text then translate to English
    const int NO_TIME_STAMPS = 50363;
    const int START_TIME = 50364;

    int numSamples;
    string[] tokens;

    int tokenCount = 0;
    NativeArray<int> outputTokens;

    // Used for special character decoding
    int[] whiteSpaceCharacters = new int[256];

    Tensor<float> encodedAudio;

    bool transcribe = false;
    string outputString = "";

    // Maximum size of audioClip (30s at 16kHz)
    const int maxSamples = 30 * 16000;

    public ModelAsset audioDecoder1, audioDecoder2;
    public ModelAsset audioEncoder;
    public ModelAsset logMelSpectro;

    // Mes variables à moi
    public TMP_Dropdown choix_langues;
    private static readonly int[] CODE_LANGUES_WHISPER = new int[]
        { // FR,EN,IT,ES,DE -> je les ai trouvés sur Hugging Face -> OpenAI -> Whisper -> added_tokens.json, *tout en bas*.
           50265,50259,50274,50262,50261
        };
    private int preferred_language = 50265;
    private bool enregistrement_en_cours = false;
    private GameObject bouton_menu;

    /*On démarre le moteur d'inférence, on ne le fait qu'une fois parce que la complexité temporelle est grande (Worker). En plus, ces structures sont réutilisables. */
    private void Awake()
    {
        SetupWhiteSpaceShifts();
        GetTokens();

        decoder1 = new Worker(ModelLoader.Load(audioDecoder1), BackendType.GPUCompute);
        decoder2 = new Worker(ModelLoader.Load(audioDecoder2), BackendType.GPUCompute);

        // Ce graphe permet de calculer la probabilité de tous les jetons et avec ArgMax on récupère le jeton avec la plus haute valeur.
        FunctionalGraph graph = new();
        var input = graph.AddInput(DataType.Float, new DynamicTensorShape(1, 1, 51865));
        var amax = Functional.ArgMax(input, -1, false);
        var selectTokenModel = graph.Compile(amax);
        argmax = new Worker(selectTokenModel, BackendType.GPUCompute);

        encoder = new Worker(ModelLoader.Load(audioEncoder), BackendType.GPUCompute);
        spectrogram = new Worker(ModelLoader.Load(logMelSpectro), BackendType.GPUCompute);
        if (choix_langues != null)
        {
            choix_langues.onValueChanged.AddListener(SetLangueUtilisee);
        }

        if (Microphone.devices.Length > 0)
        {
            microphone_name = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("Aucun microphone trouvé. Veuillez vérifier votre matériel.");
            return;
        }
        bouton_menu = GameObject.FindWithTag("Micro");
    }

    public void Start()
    {
        outputTokens = new NativeArray<int>(maxTokens, Allocator.Persistent);
        tokensTensor = new Tensor<int>(new TensorShape(1, maxTokens));
        ComputeTensorData.Pin(tokensTensor);
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);

        lastToken = new NativeArray<int>(1, Allocator.Persistent); lastToken[0] = NO_TIME_STAMPS;
        lastTokenTensor = new Tensor<int>(new TensorShape(1, 1), new[] { NO_TIME_STAMPS });
    }
    Awaitable m_Awaitable;

    NativeArray<int> lastToken;
    Tensor<int> lastTokenTensor;
    Tensor<int> tokensTensor;
    Tensor<float> audioInput;

    void LoadAudio(int echantillons_reels)
    {
        var data = new float[maxSamples];
        audioClip.GetData(data, 0);
        audioInput = new Tensor<float>(new TensorShape(1, maxSamples), data);
    }

    async Task EncodeAudio()
    {
        Debug.Log("EncodeAudio() Entrée.");
        if(encodedAudio != null)
        {
            encodedAudio.Dispose();
            encodedAudio = null;
        }

        spectrogram.Schedule(audioInput);

        await Task.Yield();
        var logmel = spectrogram.PeekOutput() as Tensor<float>;
        Debug.Log("EncodeAudio() : Création du spectrogramme.");

        encoder.Schedule(logmel);
        Debug.Log("EncodeAudio() : Encodage.");
        encodedAudio = encoder.PeekOutput() as Tensor<float>;
    }
    async Awaitable InferenceStep()
    {
        decoder1.SetInput("input_ids", tokensTensor);
        decoder1.SetInput("encoder_hidden_states", encodedAudio);
        decoder1.Schedule();

        var past_key_values_0_decoder_key = decoder1.PeekOutput("present.0.decoder.key") as Tensor<float>;
        var past_key_values_0_decoder_value = decoder1.PeekOutput("present.0.decoder.value") as Tensor<float>;
        var past_key_values_1_decoder_key = decoder1.PeekOutput("present.1.decoder.key") as Tensor<float>;
        var past_key_values_1_decoder_value = decoder1.PeekOutput("present.1.decoder.value") as Tensor<float>;
        var past_key_values_2_decoder_key = decoder1.PeekOutput("present.2.decoder.key") as Tensor<float>;
        var past_key_values_2_decoder_value = decoder1.PeekOutput("present.2.decoder.value") as Tensor<float>;
        var past_key_values_3_decoder_key = decoder1.PeekOutput("present.3.decoder.key") as Tensor<float>;
        var past_key_values_3_decoder_value = decoder1.PeekOutput("present.3.decoder.value") as Tensor<float>;

        var past_key_values_0_encoder_key = decoder1.PeekOutput("present.0.encoder.key") as Tensor<float>;
        var past_key_values_0_encoder_value = decoder1.PeekOutput("present.0.encoder.value") as Tensor<float>;
        var past_key_values_1_encoder_key = decoder1.PeekOutput("present.1.encoder.key") as Tensor<float>;
        var past_key_values_1_encoder_value = decoder1.PeekOutput("present.1.encoder.value") as Tensor<float>;
        var past_key_values_2_encoder_key = decoder1.PeekOutput("present.2.encoder.key") as Tensor<float>;
        var past_key_values_2_encoder_value = decoder1.PeekOutput("present.2.encoder.value") as Tensor<float>;
        var past_key_values_3_encoder_key = decoder1.PeekOutput("present.3.encoder.key") as Tensor<float>;
        var past_key_values_3_encoder_value = decoder1.PeekOutput("present.3.encoder.value") as Tensor<float>;

        decoder2.SetInput("input_ids", lastTokenTensor);
        decoder2.SetInput("past_key_values.0.decoder.key", past_key_values_0_decoder_key);
        decoder2.SetInput("past_key_values.0.decoder.value", past_key_values_0_decoder_value);
        decoder2.SetInput("past_key_values.1.decoder.key", past_key_values_1_decoder_key);
        decoder2.SetInput("past_key_values.1.decoder.value", past_key_values_1_decoder_value);
        decoder2.SetInput("past_key_values.2.decoder.key", past_key_values_2_decoder_key);
        decoder2.SetInput("past_key_values.2.decoder.value", past_key_values_2_decoder_value);
        decoder2.SetInput("past_key_values.3.decoder.key", past_key_values_3_decoder_key);
        decoder2.SetInput("past_key_values.3.decoder.value", past_key_values_3_decoder_value);

        decoder2.SetInput("past_key_values.0.encoder.key", past_key_values_0_encoder_key);
        decoder2.SetInput("past_key_values.0.encoder.value", past_key_values_0_encoder_value);
        decoder2.SetInput("past_key_values.1.encoder.key", past_key_values_1_encoder_key);
        decoder2.SetInput("past_key_values.1.encoder.value", past_key_values_1_encoder_value);
        decoder2.SetInput("past_key_values.2.encoder.key", past_key_values_2_encoder_key);
        decoder2.SetInput("past_key_values.2.encoder.value", past_key_values_2_encoder_value);
        decoder2.SetInput("past_key_values.3.encoder.key", past_key_values_3_encoder_key);
        decoder2.SetInput("past_key_values.3.encoder.value", past_key_values_3_encoder_value);

        decoder2.Schedule();

        var logits = decoder2.PeekOutput("logits") as Tensor<float>;
        argmax.Schedule(logits);
        using var t_Token = await argmax.PeekOutput().ReadbackAndCloneAsync() as Tensor<int>;
        int index = t_Token[0];

        outputTokens[tokenCount] = lastToken[0];
        lastToken[0] = index;
        tokenCount++;
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);
        lastTokenTensor.dataOnBackend.Upload<int>(lastToken, 1);

        if (index == END_OF_TEXT)
        {
            transcribe = false;
        }
        else if (index < tokens.Length)
        {
            outputString += GetUnicodeText(tokens[index]);
        }

        Debug.Log($"RunWhisper() : {outputString}");
    }

    // Tokenizer
    public TextAsset vocabAsset;

    /* Cette fonction récupère les tokens présents dans le fichier vocab.json*/
    void GetTokens()
    {
        var vocab = JsonConvert.DeserializeObject<Dictionary<string, int>>(vocabAsset.text);
        tokens = new string[vocab.Count];
        foreach (var item in vocab)
        {
            tokens[item.Value] = item.Key;
        }
    }

    string GetUnicodeText(string text)
    {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(ShiftCharacterDown(text));
        return Encoding.UTF8.GetString(bytes);
    }

    string ShiftCharacterDown(string text)
    {
        string outText = "";
        foreach (char letter in text)
        {
            outText += ((int)letter <= 256) ? letter : (char)whiteSpaceCharacters[(int)(letter - 256)];
        }
        return outText;
    }

    void SetupWhiteSpaceShifts()
    {
        for (int i = 0, n = 0; i < 256; i++)
        {
            if (IsWhiteSpace((char)i)) whiteSpaceCharacters[n++] = i;
        }
    }

    bool IsWhiteSpace(char c)
    {
        return !(('!' <= c && c <= '~') || ('�' <= c && c <= '�') || ('�' <= c && c <= '�'));
    }

    public void StartMicrophoneInference()
    {
        if(audioInput != null)
        {
            audioInput.Dispose();
            audioInput = null;
        }

        if (Microphone.IsRecording(microphone_name))
        {
            Microphone.End(microphone_name);
        }

        audioClip = Microphone.Start(microphone_name, false, (int)duree_defaut, 16000);
        outputString = string.Empty;

        for (int i = 0; i < maxTokens; i++)
        {
            outputTokens[i] = 0; // Vider l'historique des tokens
        }

        tokenCount = 0;

        Debug.Log("Enregistrement démarré. Durée max: " + duree_defaut + "s.");

        Invoke(nameof(StopMicrophoneAndStartInference), duree_defaut);
    }

    private async void StopMicrophoneAndStartInference()
    {
        if (!enregistrement_en_cours)
            return;

        CancelInvoke(nameof(StopMicrophoneAndStartInference));
        int position_arret = Microphone.GetPosition(microphone_name);
        Microphone.End(microphone_name);

        if(bouton_menu != null)
        {
            bouton_menu.GetComponent<AgrandissementBoutons>().SetMicroActif(false);
            bouton_menu.GetComponent<AgrandissementBoutons>().RetrecirMicro();
        }

        await Task.Delay(100);

        Debug.Log($"Enregistrement terminé. Lancement de l'inférence... avec une position égale à : {position_arret}");

        if (audioClip != null)
        {
            LoadAudio(position_arret);

            await Task.Yield();
            await EncodeAudio();
            transcribe = true;

            StartTranscriptionLoop();
        }
        else
        {
            Debug.LogError("Échec de l'enregistrement de l'AudioClip.");
        }
    }

    public async void StartTranscriptionLoop()
    {
        outputTokens[0] = START_OF_TRANSCRIPT;
        outputTokens[1] = preferred_language; //FRENCH; //ENGLISH; //...
        outputTokens[2] = TRANSCRIBE; //TRANSLATE;//
        //outputTokens[3] = NO_TIME_STAMPS;// START_TIME;//
        tokenCount = 3;
        tokensTensor.Reshape(new TensorShape(1, tokenCount));
        tokensTensor.dataOnBackend.Upload<int>(outputTokens, tokenCount);
        lastToken[0] = NO_TIME_STAMPS;
        lastTokenTensor.dataOnBackend.Upload<int>(lastToken, 1);
        while (true)
        {
            if (!transcribe || tokenCount >= (outputTokens.Length - 1))
            {
                OnTranscriptionComplete?.Invoke(outputString);
                return;
            }
            m_Awaitable = InferenceStep();
            await m_Awaitable;
        }
    }

    public int GetPreferredLanguage(int indice)
    {
        return CODE_LANGUES_WHISPER[indice];
    }

    /*@brief La procédure SetLangueUtilisee() permet de mettre à jour le langage reconnu par le modèle Whisper. Lors d'un enregistrement audio, le modèle cherchera à déterminer des mots de la langue passée en paramètre.
     @param index, un entier index qui est relié au tableau constant CODE_LANGUE_WHISPER.
    La fonction met à jour preferred_language vers une nouvelle valeur (par défaut sur français.*/
    public void SetLangueUtilisee(int index)
    {
        Debug.Log($"SetLangueUtilisee() Valeur du paramètre : {index}");
        preferred_language = GetPreferredLanguage(index);
    }

    public void DemarrerEnregistrement()
    {
        // Si on est déjà en train de calculer, on ignore le clic pour éviter les bugs
        //if (enregistrement_en_cours) return;

        Debug.Log($"Clic Micro. Enregistrement en cours : {enregistrement_en_cours}");

        if (enregistrement_en_cours)
        {
            // On lance la version asynchrone (sans 'await' ici car DemarrerEnregistrement est void)
            StopMicrophoneAndStartInference();
            enregistrement_en_cours = false;
        }
        else
        {
            StartMicrophoneInference();
            enregistrement_en_cours = true;
        }
    }

    private void OnDestroy()
    {
        decoder1.Dispose();
        decoder2.Dispose();
        encoder.Dispose();
        spectrogram.Dispose();
        argmax.Dispose();
        audioInput.Dispose();
        lastTokenTensor.Dispose();
        tokensTensor.Dispose();
    }
}
