using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFeedbackManager : MonoBehaviour
{
    public static HitFeedbackManager Instance { get; private set; }

    [Header("Feedback Text Prefab")]
    [Tooltip("Prefab tulisan HIT yang muncul saat note berhasil.")]
    [SerializeField]
    private GameObject hitTextPrefab;

    [Tooltip("Prefab tulisan MISS yang muncul saat note gagal.")]
    [SerializeField]
    private GameObject missTextPrefab;

    [Tooltip("Durasi prefab tulisan sebelum dihancurkan.")]
    [SerializeField, Min(0.1f)]
    private float textLifetime = 0.7f;

    [Tooltip("Offset posisi tulisan dari posisi particle.")]
    [SerializeField]
    private Vector3 textPositionOffset =
        new Vector3(0f, 0.5f, 0f);

    [Tooltip("Membuat tulisan menghadap kamera saat dibuat.")]
    [SerializeField]
    private bool textFacesCamera = true;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;

    [Tooltip("Suara dup ketika note berhasil dipukul.")]
    [SerializeField]
    private AudioClip hitSound;

    [Range(0f, 1f)]
    [SerializeField]
    private float hitSoundVolume = 1f;

    [Header("Hit Particle")]
    [Tooltip(
        "Gunakan particle prefab berwarna putih agar dapat mengikuti warna note.")]
    [SerializeField]
    private ParticleSystem hitParticlePrefab;

    [Tooltip("Offset particle dari titik efek lane.")]
    [SerializeField]
    private Vector3 particlePositionOffset = Vector3.zero;

    [Header("Lane Environment")]
    [Tooltip(
        "Masukkan empat environment: LeftA, LeftB, RightA, dan RightB.")]
    [SerializeField]
    private List<LaneEnvironmentData> laneEnvironments = new();

    [Tooltip("Warna normal environment sebelum dan setelah HIT.")]
    [SerializeField]
    private Color defaultEnvironmentColor = Color.white;

    [Tooltip("Berapa lama environment memakai warna note.")]
    [SerializeField, Min(0.05f)]
    private float environmentColorDuration = 0.4f;

    [Tooltip(
        "Kekuatan cahaya emission material. Gunakan 0 jika tidak diperlukan.")]
    [SerializeField, Min(0f)]
    private float emissionIntensity = 2f;

    private readonly Dictionary<LaneType, LaneEnvironmentData>
        laneLookup = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        PrepareAudioSource();
        BuildLaneLookup();
    }

    private void PrepareAudioSource()
    {
        if (audioSource != null)
            return;

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError(
                "HitFeedbackManager: AudioSource belum dipasang.");
        }
    }

    private void BuildLaneLookup()
    {
        laneLookup.Clear();

        foreach (LaneEnvironmentData data in laneEnvironments)
        {
            if (data == null)
                continue;

            if (laneLookup.ContainsKey(data.lane))
            {
                Debug.LogError(
                    $"HitFeedbackManager: Lane {data.lane} " +
                    "dimasukkan lebih dari satu kali.");

                continue;
            }

            PrepareEnvironment(data);
            laneLookup.Add(data.lane, data);

            ApplyEnvironmentColor(
                data,
                defaultEnvironmentColor);
        }
    }

    private void PrepareEnvironment(
        LaneEnvironmentData data)
    {
        if (data.environmentRenderer == null)
            return;

        data.runtimeMaterial =
            data.environmentRenderer.material;
    }

    public void PlayHitFeedback(
    LaneType lane,
    Color noteColor,
    Vector3 noteWorldPosition)
    {
        Vector3 effectPosition =
            GetEffectPosition(
                lane,
                noteWorldPosition);

        SpawnTextPrefab(
            hitTextPrefab,
            effectPosition);

        PlaySound(
            hitSound,
            hitSoundVolume);

        SpawnHitParticle(
            effectPosition,
            noteColor);

        if (laneLookup.TryGetValue(
            lane,
            out LaneEnvironmentData environmentData))
        {
            FlashEnvironment(
                environmentData,
                noteColor);
        }
    }

    public void PlayMissFeedback(
        LaneType lane,
        Vector3 noteWorldPosition)
    {
        Vector3 effectPosition =
            GetEffectPosition(
                lane,
                noteWorldPosition);

        SpawnTextPrefab(
            missTextPrefab,
            effectPosition);
    }

    private Vector3 GetEffectPosition(
        LaneType lane,
        Vector3 fallbackPosition)
    {
        if (laneLookup.TryGetValue(
            lane,
            out LaneEnvironmentData environmentData))
        {
            if (environmentData.effectSpawnPoint != null)
            {
                return environmentData
                    .effectSpawnPoint
                    .position;
            }
        }

        return fallbackPosition;
    }

    // =========================================================
    // TEXT PREFAB
    // =========================================================

    private void SpawnTextPrefab(
        GameObject textPrefab,
        Vector3 effectPosition)
    {
        if (textPrefab == null)
            return;

        Vector3 spawnPosition =
            effectPosition + textPositionOffset;

        Quaternion spawnRotation =
            textPrefab.transform.rotation;

        if (textFacesCamera && Camera.main != null)
        {
            spawnRotation =
                Camera.main.transform.rotation;
        }

        GameObject spawnedText = Instantiate(
            textPrefab,
            spawnPosition,
            spawnRotation);

        Destroy(
            spawnedText,
            textLifetime);
    }

    private void PlaySound(
        AudioClip clip,
        float volume)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(
            clip,
            volume);
    }

    private void SpawnHitParticle(
        Vector3 effectPosition,
        Color particleColor)
    {
        if (hitParticlePrefab == null)
            return;

        Vector3 spawnPosition =
            effectPosition + particlePositionOffset;

        ParticleSystem particle = Instantiate(
            hitParticlePrefab,
            spawnPosition,
            hitParticlePrefab.transform.rotation);

        ParticleSystem.MainModule main =
            particle.main;

        // Particle mengikuti warna note.
        main.startColor = particleColor;

        particle.Play();

        float destroyDelay =
            main.duration +
            main.startLifetime.constantMax;

        Destroy(
            particle.gameObject,
            Mathf.Max(0.1f, destroyDelay));
    }

    private void FlashEnvironment(
        LaneEnvironmentData environmentData,
        Color noteColor)
    {
        if (environmentData.resetCoroutine != null)
        {
            StopCoroutine(
                environmentData.resetCoroutine);
        }

        ApplyEnvironmentColor(
            environmentData,
            noteColor);

        environmentData.resetCoroutine =
            StartCoroutine(
                ResetEnvironmentAfterDelay(
                    environmentData));
    }

    private IEnumerator ResetEnvironmentAfterDelay(
        LaneEnvironmentData environmentData)
    {
        yield return new WaitForSecondsRealtime(
            environmentColorDuration);

        ApplyEnvironmentColor(
            environmentData,
            defaultEnvironmentColor);

        environmentData.resetCoroutine = null;
    }

    private void ApplyEnvironmentColor(
        LaneEnvironmentData environmentData,
        Color targetColor)
    {
        Renderer environmentRenderer =
            environmentData.environmentRenderer;

        if (environmentRenderer != null)
        {
            /*
             * Jika menggunakan SpriteRenderer,
             * ubah warna SpriteRenderer secara langsung.
             */
            if (environmentRenderer
                is SpriteRenderer spriteRenderer)
            {
                spriteRenderer.color = targetColor;
            }
            else
            {
                ApplyMaterialColor(
                    environmentData.runtimeMaterial,
                    targetColor);
            }
        }

        if (environmentData.environmentLight != null)
        {
            environmentData.environmentLight.color =
                targetColor;
        }
    }

    private void ApplyMaterialColor(
        Material material,
        Color targetColor)
    {
        if (material == null)
            return;

        // URP Lit.
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor(
                "_BaseColor",
                targetColor);
        }

        else if (material.HasProperty("_Color"))
        {
            material.SetColor(
                "_Color",
                targetColor);
        }

        if (material.HasProperty("_EmissionColor"))
        {
            if (emissionIntensity > 0f)
            {
                material.EnableKeyword("_EMISSION");

                material.SetColor(
                    "_EmissionColor",
                    targetColor * emissionIntensity);
            }
            else
            {
                material.SetColor(
                    "_EmissionColor",
                    Color.black);
            }
        }
    }
}

[Serializable]
public class LaneEnvironmentData
{
    [Header("Lane")]
    public LaneType lane;

    [Header("Environment")]
    [Tooltip("Renderer dari environment lane ini.")]
    public Renderer environmentRenderer;

    [Tooltip("Opsional. Light yang warnanya ikut berubah.")]
    public Light environmentLight;

    [Header("Effect Position")]
    [Tooltip(
        "Posisi munculnya particle serta prefab HIT/MISS.")]
    public Transform effectSpawnPoint;

    [NonSerialized]
    public Material runtimeMaterial;

    [NonSerialized]
    public Coroutine resetCoroutine;
}