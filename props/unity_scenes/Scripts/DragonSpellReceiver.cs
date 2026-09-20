using System;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class DragonSpellReceiver : MonoBehaviour
{
    [Header("MQTT Broker Configuration")]
    public string brokerAddress = "<YOUR_HOME_ASSISTANT_IP>";
    public int brokerPort = 1883;
    public string mqttUser = "<YOUR_MQTT_USERNAME>";
    public string mqttPass = "<YOUR_MQTT_PASSWORD>";

    [Header("Dragon Target")]
    public Animator dragonAnimator;
    public DragonCombatManager combatManager; // LINK THIS IN THE INSPECTOR

    [Header("Optional Spell Effects & Audio")]
    public ParticleSystem lightningImpactVFX;
    public ParticleSystem fireballImpactVFX;
    public AudioSource spellAudioSource;
    public AudioClip lightningSFX;
    public AudioClip fireballSFX;

    private MqttClient client;

    [Serializable]
    public class WandPayload
    {
        public string wand_id;
        public int wand_id_dec;
        public int magnitude;
        public string sensor_id;
        public string spell;
    }

    void Start()
    {
        Application.runInBackground = true;
        ConnectToBroker();
    }

    void ConnectToBroker()
    {
        try
        {
            client = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
            client.MqttMsgPublishReceived += OnMessageReceived;

            string clientId = "Unity_Dragon_" + Guid.NewGuid().ToString().Substring(0, 5);
            client.Connect(clientId, mqttUser, mqttPass);

            client.Subscribe(new string[] { "mythichome/events/cast/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            Debug.Log("<color=green>[MQTT]</color> Connected and listening for wand casts!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MQTT] Connection failed: {ex.Message}");
        }
    }

    void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string topic = e.Topic;
        string json = Encoding.UTF8.GetString(e.Message);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            HandleSpellCast(topic, json);
        });
    }

    void HandleSpellCast(string topic, string json)
    {
        Debug.Log($"[Spell Cast Received] Topic: {topic} | Data: {json}");

        // Handle game exit command
        if (topic.EndsWith("quit") || topic.EndsWith("stop"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return;
        }

        // 1. Check if the dragon is already dead. If so, reset the scene and exit early!
        if (combatManager != null && combatManager.IsDead)
        {
            combatManager.ResetScene();
            return;
        }

        WandPayload data = JsonUtility.FromJson<WandPayload>(json);

        // 2. Apply damage and check if this spell was the killing blow
        bool killedDragon = false;
        if (combatManager != null)
        {
            killedDragon = combatManager.TakeDamage(1);
        }

        // 3. Play the spell effects
        if (topic.EndsWith("lightning"))
        {
            OnLightningCast(data.magnitude, killedDragon);
        }
        else if (topic.EndsWith("fireball"))
        {
            OnFireballCast(data.magnitude, killedDragon);
        }
    }

    void OnLightningCast(int magnitude, bool killedDragon)
    {
        // Only play the normal hit animation if it didn't die (CombatManager handles the Die animation)
        if (!killedDragon && dragonAnimator) dragonAnimator.SetTrigger("HitLightning");

        if (lightningImpactVFX) lightningImpactVFX.Play();
        if (spellAudioSource && lightningSFX) spellAudioSource.PlayOneShot(lightningSFX);
    }

    void OnFireballCast(int magnitude, bool killedDragon)
    {
        if (!killedDragon && dragonAnimator) dragonAnimator.SetTrigger("HitFireball");

        if (fireballImpactVFX) fireballImpactVFX.Play();
        if (spellAudioSource && fireballSFX) spellAudioSource.PlayOneShot(fireballSFX);
    }

    void OnApplicationQuit()
    {
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
        }
    }
}