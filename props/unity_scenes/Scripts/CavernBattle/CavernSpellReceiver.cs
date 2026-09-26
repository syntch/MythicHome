using System;
using System.Text;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class CavernSpellReceiver : MonoBehaviour
{
    [Header("MQTT Broker Configuration")]
    public string brokerAddress = "<YOUR_HOME_ASSISTANT_IP>";
    public int brokerPort = 1883;
    public string mqttUser = "<YOUR_MQTT_USERNAME>";
    public string mqttPass = "<YOUR_MQTT_PASSWORD>";

    [Header("MQTT Topics")]
    public string spellSelectTopic = "mythichome/player/spell_select";
    public string wandCastTopic = "mythichome/events/cast/#";

    [Header("Combat Reference")]
    public CavernCombatManager combatManager;

    private MqttClient client;

    [Serializable]
    public class WandPayload
    {
        public string wand_id;
        public int wand_id_dec;
        public int magnitude;
        public string sensor_id;
        public string spell_type;
    }

    [Serializable]
    public class SpellSelectPayload
    {
        public string spell;
    }

    void Start()
    {
        Application.runInBackground = true;
        ConnectToBroker();
    }

    void Update()
    {
        // Built-in keyboard shortcuts for quick testing inside the Unity Editor
        if (combatManager == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) combatManager.SelectSpell("fireball");
        if (Input.GetKeyDown(KeyCode.Alpha2)) combatManager.SelectSpell("lightning");
        if (Input.GetKeyDown(KeyCode.Alpha3)) combatManager.SelectSpell("ice_spear");
        if (Input.GetKeyDown(KeyCode.Alpha4)) combatManager.SelectSpell("shield");
        if (Input.GetKeyDown(KeyCode.Space))  combatManager.OnWandCast(220);
    }

    void ConnectToBroker()
    {
        try
        {
            client = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
            client.MqttMsgPublishReceived += OnMessageReceived;

            string clientId = "Unity_CavernDragon_" + Guid.NewGuid().ToString().Substring(0, 5);
            client.Connect(clientId, mqttUser, mqttPass);

            client.Subscribe(
                new string[] { spellSelectTopic, wandCastTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE }
            );

            Debug.Log("<color=cyan>[MQTT Cavern]</color> Connected! Listening for Phone Spellbook & Wand Casts.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MQTT Cavern] Connection failed: {ex.Message}");
        }
    }

    void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string topic = e.Topic;
        string rawPayload = Encoding.UTF8.GetString(e.Message);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            HandleIncomingMessage(topic, rawPayload);
        });
    }

    void HandleIncomingMessage(string topic, string payload)
    {
        Debug.Log($"[Cavern MQTT] Topic: {topic} | Payload: {payload}");

        // 1. Handle game exit command
        if (topic.EndsWith("quit") || topic.EndsWith("stop"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return;
        }

        if (combatManager == null) return;

        // 2. Handle Spell Selection from Phone Dashboard ("mythichome/player/spell_select")
        if (topic == spellSelectTopic)
        {
            string selectedSpell = payload.Trim().Trim('"');
            if (selectedSpell.StartsWith("{"))
            {
                try
                {
                    SpellSelectPayload parsed = JsonUtility.FromJson<SpellSelectPayload>(selectedSpell);
                    if (!string.IsNullOrEmpty(parsed.spell))
                        selectedSpell = parsed.spell;
                }
                catch { /* Fallback to raw string */ }
            }

            combatManager.SelectSpell(selectedSpell);
            return;
        }

        // 3. Handle Wand Flick Cast from ESP32 Sensor ("mythichome/events/cast/#")
        int magnitude = 150;
        if (payload.TrimStart().StartsWith("{"))
        {
            try
            {
                WandPayload wandData = JsonUtility.FromJson<WandPayload>(payload);
                magnitude = wandData.magnitude;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Cavern MQTT] Failed to parse WandPayload JSON, using default magnitude: {ex.Message}");
            }
        }

        combatManager.OnWandCast(magnitude);
    }

    void OnApplicationQuit()
    {
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
        }
    }
}
