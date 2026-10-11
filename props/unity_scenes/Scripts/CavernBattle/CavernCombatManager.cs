using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CavernCombatManager : MonoBehaviour
{
    public enum SpellType
    {
        Fireball,
        Lightning,
        IceSpear,
        Shield
    }

    [Header("Dragon Health & Shield Settings")]
    public int maxDragonHealth = 12;
    private int currentDragonHealth;
    public bool IsDragonDead { get; private set; } = false;
    public bool IsDragonShielded { get; private set; } = false;

    [Tooltip("Dragon automatically casts its shield at 66% and 33% HP, plus periodic intervals.")]
    public float dragonShieldCooldown = 25f;
    private float shieldTimer = 0f;
    private bool triggered66Shield = false;
    private bool triggered33Shield = false;

    [Header("Dragon Attack Settings")]
    public float minAttackInterval = 14f;
    public float maxAttackInterval = 22f;
    public float attackWindupSeconds = 5.0f;
    private float nextAttackTimer = 0f;
    private bool isDragonWindingUpAttack = false;
    private float currentWindupRemaining = 0f;

    [Header("Player Health & Shield Settings")]
    public int maxPlayerHealth = 3;
    private int currentPlayerHealth;
    public bool IsPlayerDead { get; private set; } = false;
    public bool IsPlayerShielded { get; private set; } = false;
    public float playerShieldDuration = 6.0f;
    private float playerShieldRemaining = 0f;

    [Header("Currently Equipped Spell (From Phone)")]
    public SpellType currentEquippedSpell = SpellType.Fireball;

    [Header("UI Elements")]
    public Slider dragonHealthBar;
    public Slider playerHealthBar;
    public TextMeshProUGUI equippedSpellText;
    public TextMeshProUGUI combatWarningText;
    public TextMeshProUGUI victoryDefeatText;

    [Header("Dragon Animator")]
    public Animator dragonAnimator;
    private static readonly int DieHash = Animator.StringToHash("DieTrigger");
    private static readonly int ScreamHash = Animator.StringToHash("ScreamTrigger");
    private static readonly int AttackHash = Animator.StringToHash("AttackTrigger");
    private static readonly int HitLightningHash = Animator.StringToHash("HitLightning");
    private static readonly int HitFireballHash = Animator.StringToHash("HitFireball");

    [Header("Visual Effects (VFX)")]
    public GameObject dragonShieldVisual;        // Persistent aura object enabled while Dragon is shielded
    public ParticleSystem dragonShieldBreakVFX;  // Burst played when Ice Spear shatters Dragon shield
    public GameObject playerShieldVisual;        // Screen border / barrier object enabled while Player is shielded
    public ParticleSystem fireballImpactVFX;
    public ParticleSystem lightningImpactVFX;
    public ParticleSystem iceSpearImpactVFX;
    public ParticleSystem dragonAttackVFX;

    [Header("Audio Effects (SFX)")]
    public AudioSource spellAudioSource;
    public AudioClip equipSpellSFX;
    public AudioClip fireballSFX;
    public AudioClip lightningSFX;
    public AudioClip iceSpearSFX;
    public AudioClip shieldCastSFX;
    public AudioClip shieldDeflectSFX;
    public AudioClip shieldShatterSFX;
    public AudioClip dragonTelegraphSFX;
    public AudioClip dragonAttackSFX;
    public AudioClip playerHurtSFX;

    public bool IsMatchOver => IsDragonDead || IsPlayerDead;

    void Awake()
    {
        // Immediately clear any serialized placeholder or emoji text so TextMeshPro doesn't warn on scene load
        if (victoryDefeatText != null)
        {
            victoryDefeatText.text = "";
            victoryDefeatText.gameObject.SetActive(false);
        }

        if (combatWarningText != null)
            combatWarningText.text = "";
    }

    void Start()
    {
        currentDragonHealth = maxDragonHealth;
        currentPlayerHealth = maxPlayerHealth;

        if (dragonHealthBar != null)
        {
            dragonHealthBar.maxValue = maxDragonHealth;
            dragonHealthBar.value = currentDragonHealth;
        }

        if (playerHealthBar != null)
        {
            playerHealthBar.maxValue = maxPlayerHealth;
            playerHealthBar.value = currentPlayerHealth;
        }

        if (victoryDefeatText != null)
        {
            victoryDefeatText.text = "";
            victoryDefeatText.gameObject.SetActive(false);
        }

        if (combatWarningText != null)
            combatWarningText.text = "";

        DisableShieldVisual(dragonShieldVisual);
        DisableShieldVisual(playerShieldVisual);

        StopVFX(fireballImpactVFX);
        StopVFX(lightningImpactVFX);
        StopVFX(iceSpearImpactVFX);
        StopVFX(dragonShieldBreakVFX);
        StopVFX(dragonAttackVFX);

        ScheduleNextDragonAttack();
        shieldTimer = dragonShieldCooldown;
        UpdateEquippedSpellUI();
    }

    private void EnableShieldVisual(GameObject shieldObj)
    {
        if (shieldObj == null) return;

        // Ensure the shield GameObject and any inactive parents are active
        Transform curr = shieldObj.transform;
        while (curr != null)
        {
            if (!curr.gameObject.activeSelf)
                curr.gameObject.SetActive(true);
            curr = curr.parent;
        }

        // Trigger playback on all ParticleSystems on the shield and its children
        ParticleSystem[] systems = shieldObj.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            ps.Play(true);
        }
    }

    private void DisableShieldVisual(GameObject shieldObj)
    {
        if (shieldObj == null) return;

        // Stop and clear all ParticleSystems on the shield
        ParticleSystem[] systems = shieldObj.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        shieldObj.SetActive(false);
    }

    private void StopVFX(ParticleSystem ps)
    {
        if (ps == null) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        if (IsMatchOver) return;

        // 1. Update Player Shield Timer
        if (IsPlayerShielded)
        {
            playerShieldRemaining -= Time.deltaTime;
            if (playerShieldRemaining <= 0f)
            {
                DeactivatePlayerShield();
            }
        }

        // 2. Update Periodic Dragon Shield Timer (if not currently shielded or attacking)
        if (!IsDragonShielded && !isDragonWindingUpAttack)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                ActivateDragonShield();
                shieldTimer = dragonShieldCooldown;
            }
        }

        // 3. Update Dragon Attack Wind-up or Cooldown
        if (isDragonWindingUpAttack)
        {
            currentWindupRemaining -= Time.deltaTime;
            if (combatWarningText != null)
            {
                string shieldStatus = IsPlayerShielded
                    ? "<color=#00E5FF><b>[SHIELD READY]</b></color>"
                    : "<color=#FF5252><b>Cast SHIELD Now!</b></color>";
                combatWarningText.text = $"<color=#FF5252><b>INCOMING DRAGON BREATH ({Mathf.Max(0f, currentWindupRemaining):F1}s)!</b></color>\n{shieldStatus}";
            }

            if (currentWindupRemaining <= 0f)
            {
                ExecuteDragonAttack();
            }
        }
        else
        {
            nextAttackTimer -= Time.deltaTime;
            if (nextAttackTimer <= 0f)
            {
                StartDragonAttackWindup();
            }
        }
    }

    // =========================================================================
    // SPELL EQUIPPING (FROM HOME ASSISTANT PHONE DASHBOARD)
    // =========================================================================

    public void SelectSpell(string spellIdentifier)
    {
        if (string.IsNullOrEmpty(spellIdentifier)) return;

        string normalized = spellIdentifier.Trim().ToLowerInvariant();
        if (normalized.Contains("fire"))
            currentEquippedSpell = SpellType.Fireball;
        else if (normalized.Contains("light"))
            currentEquippedSpell = SpellType.Lightning;
        else if (normalized.Contains("ice") || normalized.Contains("spear"))
            currentEquippedSpell = SpellType.IceSpear;
        else if (normalized.Contains("shield") || normalized.Contains("ward"))
            currentEquippedSpell = SpellType.Shield;

        if (spellAudioSource != null && equipSpellSFX != null)
            spellAudioSource.PlayOneShot(equipSpellSFX);

        UpdateEquippedSpellUI();
        Debug.Log($"[CavernCombat] Equipped spell changed to: {currentEquippedSpell}");
    }

    private void UpdateEquippedSpellUI()
    {
        if (equippedSpellText == null) return;

        switch (currentEquippedSpell)
        {
            case SpellType.Fireball:
                equippedSpellText.text = "Equipped: <color=#FF6D00><b>FIREBALL</b></color> (Heavy Attack)";
                break;
            case SpellType.Lightning:
                equippedSpellText.text = "Equipped: <color=#FFEA00><b>LIGHTNING BOLT</b></color> (Quick Strike)";
                break;
            case SpellType.IceSpear:
                equippedSpellText.text = "Equipped: <color=#00E5FF><b>ICE SPEAR</b></color> (Shield Breaker)";
                break;
            case SpellType.Shield:
                equippedSpellText.text = "Equipped: <color=#69F0AE><b>ARCANE SHIELD</b></color> (Block Attack)";
                break;
        }
    }

    // =========================================================================
    // WAND CAST EXECUTION (FROM ESP32 IR SENSOR)
    // =========================================================================

    public void OnWandCast(int magnitude)
    {
        if (IsMatchOver)
        {
            ResetScene();
            return;
        }

        bool isHeavyFlick = magnitude >= 200;

        switch (currentEquippedSpell)
        {
            case SpellType.Fireball:
                CastFireball(isHeavyFlick);
                break;
            case SpellType.Lightning:
                CastLightning(isHeavyFlick);
                break;
            case SpellType.IceSpear:
                CastIceSpear();
                break;
            case SpellType.Shield:
                ActivatePlayerShield();
                break;
        }
    }

    private void PlayVFX(ParticleSystem ps)
    {
        if (ps == null) return;

        // Ensure the particle system's GameObject and any inactive parents are active
        Transform curr = ps.transform;
        while (curr != null)
        {
            if (!curr.gameObject.activeSelf)
                curr.gameObject.SetActive(true);
            curr = curr.parent;
        }

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);
    }

    private void CastFireball(bool isHeavyFlick)
    {
        PlayVFX(fireballImpactVFX);

        if (IsDragonShielded)
        {
            if (spellAudioSource != null && shieldDeflectSFX != null)
                spellAudioSource.PlayOneShot(shieldDeflectSFX);

            ShowTemporaryWarning("Dragon's Shield deflected Fireball! Equip <color=#00E5FF><b>ICE SPEAR</b></color> to break it!", 2.5f);
            return;
        }

        if (spellAudioSource != null && fireballSFX != null)
            spellAudioSource.PlayOneShot(fireballSFX);

        int damage = isHeavyFlick ? 3 : 2;
        bool killed = ApplyDragonDamage(damage);
        if (!killed && dragonAnimator != null)
            dragonAnimator.SetTrigger(HitFireballHash);
    }

    private void CastLightning(bool isHeavyFlick)
    {
        PlayVFX(lightningImpactVFX);

        if (IsDragonShielded)
        {
            if (spellAudioSource != null && shieldDeflectSFX != null)
                spellAudioSource.PlayOneShot(shieldDeflectSFX);

            ShowTemporaryWarning("Dragon's Shield deflected Lightning! Equip <color=#00E5FF><b>ICE SPEAR</b></color> to break it!", 2.5f);
            return;
        }

        if (spellAudioSource != null && lightningSFX != null)
            spellAudioSource.PlayOneShot(lightningSFX);

        int damage = isHeavyFlick ? 2 : 1;
        bool killed = ApplyDragonDamage(damage);
        if (!killed && dragonAnimator != null)
            dragonAnimator.SetTrigger(HitLightningHash);
    }

    private void CastIceSpear()
    {
        PlayVFX(iceSpearImpactVFX);
        if (spellAudioSource != null && iceSpearSFX != null)
            spellAudioSource.PlayOneShot(iceSpearSFX);

        if (IsDragonShielded)
        {
            // Shatter the Dragon's Shield!
            IsDragonShielded = false;
            DisableShieldVisual(dragonShieldVisual);
            PlayVFX(dragonShieldBreakVFX);
            if (spellAudioSource != null && shieldShatterSFX != null)
                spellAudioSource.PlayOneShot(shieldShatterSFX);

            ShowTemporaryWarning("<color=#00E5FF><b>SHATTERED!</b></color> Dragon's Shield is broken!", 2.5f);
        }

        // Ice Spear also deals 1 piercing damage
        bool killed = ApplyDragonDamage(1);
        if (!killed && dragonAnimator != null)
            dragonAnimator.SetTrigger(HitLightningHash);
    }

    // =========================================================================
    // PLAYER SHIELD LOGIC
    // =========================================================================

    public void ActivatePlayerShield()
    {
        IsPlayerShielded = true;
        playerShieldRemaining = playerShieldDuration;

        EnableShieldVisual(playerShieldVisual);

        if (spellAudioSource != null && shieldCastSFX != null)
            spellAudioSource.PlayOneShot(shieldCastSFX);

        if (!isDragonWindingUpAttack)
        {
            ShowTemporaryWarning($"<color=#69F0AE><b>Arcane Shield Raised ({playerShieldDuration:F0}s)!</b></color>", 2.0f);
        }
    }

    private void DeactivatePlayerShield()
    {
        IsPlayerShielded = false;
        playerShieldRemaining = 0f;
        DisableShieldVisual(playerShieldVisual);
    }

    // =========================================================================
    // DRAGON SHIELD & ATTACK AI
    // =========================================================================

    public void ActivateDragonShield()
    {
        if (IsDragonDead || IsDragonShielded) return;

        IsDragonShielded = true;
        EnableShieldVisual(dragonShieldVisual);

        if (dragonAnimator != null)
            dragonAnimator.SetTrigger(ScreamHash);

        ShowTemporaryWarning("<color=#FF5252><b>DRAGON CAST FLAME ARMOR!</b></color> Select <color=#00E5FF><b>ICE SPEAR</b></color> on phone to shatter it!", 4.0f);
    }

    private void StartDragonAttackWindup()
    {
        if (IsMatchOver) return;

        isDragonWindingUpAttack = true;
        currentWindupRemaining = attackWindupSeconds;

        if (dragonAnimator != null)
            dragonAnimator.SetTrigger(ScreamHash);

        if (spellAudioSource != null && dragonTelegraphSFX != null)
            spellAudioSource.PlayOneShot(dragonTelegraphSFX);
    }

    private void ExecuteDragonAttack()
    {
        isDragonWindingUpAttack = false;
        ScheduleNextDragonAttack();

        if (dragonAnimator != null)
            dragonAnimator.SetTrigger(AttackHash);

        PlayVFX(dragonAttackVFX);

        if (spellAudioSource != null && dragonAttackSFX != null)
            spellAudioSource.PlayOneShot(dragonAttackSFX);

        if (IsPlayerShielded)
        {
            // Attack blocked!
            if (spellAudioSource != null && shieldDeflectSFX != null)
                spellAudioSource.PlayOneShot(shieldDeflectSFX);

            DeactivatePlayerShield();
            ShowTemporaryWarning("<color=#69F0AE><b>BLOCKED!</b></color> Your Arcane Shield absorbed the Dragon's breath!", 3.0f);
        }
        else
        {
            // Player takes hit!
            currentPlayerHealth -= 1;
            if (playerHealthBar != null)
                playerHealthBar.value = currentPlayerHealth;

            if (spellAudioSource != null && playerHurtSFX != null)
                spellAudioSource.PlayOneShot(playerHurtSFX);

            if (currentPlayerHealth <= 0)
            {
                PlayerDefeat();
            }
            else
            {
                ShowTemporaryWarning($"<color=#FF5252><b>HIT BY DRAGON BREATH!</b></color> ({currentPlayerHealth} HP Remaining)", 3.0f);
            }
        }
    }

    private void ScheduleNextDragonAttack()
    {
        nextAttackTimer = Random.Range(minAttackInterval, maxAttackInterval);
    }

    // =========================================================================
    // DAMAGE, VICTORY & DEFEAT
    // =========================================================================

    private bool ApplyDragonDamage(int damage)
    {
        if (IsDragonDead) return true;

        currentDragonHealth -= damage;
        if (dragonHealthBar != null)
            dragonHealthBar.value = currentDragonHealth;

        // Check HP milestone shields (66% and 33%)
        float hpRatio = (float)currentDragonHealth / maxDragonHealth;
        if (!triggered66Shield && hpRatio <= 0.66f && currentDragonHealth > 0)
        {
            triggered66Shield = true;
            ActivateDragonShield();
        }
        else if (!triggered33Shield && hpRatio <= 0.33f && currentDragonHealth > 0)
        {
            triggered33Shield = true;
            ActivateDragonShield();
        }

        if (currentDragonHealth <= 0)
        {
            DragonDeath();
            return true;
        }

        return false;
    }

    private void DragonDeath()
    {
        IsDragonDead = true;
        IsDragonShielded = false;
        isDragonWindingUpAttack = false;

        DisableShieldVisual(dragonShieldVisual);

        if (dragonAnimator != null)
        {
            // Clear any lingering combat triggers that might compete with death
            dragonAnimator.ResetTrigger(AttackHash);
            dragonAnimator.ResetTrigger(ScreamHash);
            dragonAnimator.ResetTrigger(HitLightningHash);
            dragonAnimator.ResetTrigger(HitFireballHash);

            dragonAnimator.SetTrigger(DieHash);
            // Immediately force transition into the Die animation state
            dragonAnimator.CrossFade("Die", 0.15f);
            Debug.Log("<color=green>[CavernCombat]</color> Dragon defeated! Die animation triggered.");
        }
        else
        {
            Debug.LogWarning("[CavernCombat] dragonAnimator is null on DragonDeath!");
        }

        if (combatWarningText != null)
            combatWarningText.text = "";

        if (victoryDefeatText != null)
        {
            victoryDefeatText.text = "<color=#FFD700><b>VICTORY!</b></color>\n<size=60%>Flick Wand to Play Again</size>";
            victoryDefeatText.gameObject.SetActive(true);
        }
    }

    private void PlayerDefeat()
    {
        IsPlayerDead = true;
        isDragonWindingUpAttack = false;

        if (combatWarningText != null)
            combatWarningText.text = "";

        if (victoryDefeatText != null)
        {
            victoryDefeatText.text = "<color=#FF5252><b>DEFEATED!</b></color>\n<size=60%>Flick Wand to Try Again</size>";
            victoryDefeatText.gameObject.SetActive(true);
        }
    }

    private Coroutine warningCoroutine;
    private void ShowTemporaryWarning(string message, float duration)
    {
        if (combatWarningText == null) return;
        if (warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(ClearWarningAfterDelay(message, duration));
    }

    private IEnumerator ClearWarningAfterDelay(string message, float duration)
    {
        combatWarningText.text = message;
        yield return new WaitForSeconds(duration);
        if (!isDragonWindingUpAttack && combatWarningText != null)
        {
            combatWarningText.text = IsDragonShielded
                ? "<color=#FF5252><b>DRAGON SHIELD ACTIVE</b></color> — Cast <color=#00E5FF><b>ICE SPEAR</b></color> to Shatter!"
                : "";
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
