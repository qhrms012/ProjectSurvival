using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    private void Awake()
    {
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(AudioManager.Instance.SetBgmVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
    }

    public void SetMasterVolume(float volume)
    {
        // Master ������ BGM �� SFX ��ο� ������ ��ġ�� AudioMixer �Ķ���ͷ� ����
        float adjustedVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        m_AudioMixer.SetFloat("Master", adjustedVolume);  // "Master"�� AudioMixer�� ������ �Ķ���� �̸�
    }

}