using System.Collections;
using System.Collections.Generic;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEditor;
using UnityEngine;

public partial class ProjectSetting
{
    private class AudioHandle
    {
        public bool EnableAudio;
        public float GlobalVolume;
        public float BGMVolume;
        public float SoundVolume;

        public AudioHandle(bool enableAudio, float allAudioVolume, float musicVolume, float soundVolume)
        {
            this.EnableAudio = enableAudio;
            this.GlobalVolume = allAudioVolume;
            this.BGMVolume = musicVolume;
            this.SoundVolume = soundVolume;
        }
    }
    private string m_AudioConfigPath = "/GDGFramework/Config/AudioConfig.json";
    private static bool s_EnableAudio
    {
        get => AudioManager.IsMute;
        set => AudioManager.IsMute = value;
        
    }
    private static float s_AllAudioVolume 
    {
        get => AudioManager.GlobalVolume;
        set
        {
            AudioManager.GlobalVolume = value;
            AudioManager.SetAllAudioVolume(value); 
        }
    }
    private static float s_MusicVolume
    {
        get => AudioManager.MusicVolume;
        set
        {
            AudioManager.MusicVolume = value;
            AudioManager.SetMusicVolume(value); 
        }
    }
    private static float s_SoundVolume
    {
        get => AudioManager.SoundVolume;
        set
        {
            AudioManager.SoundVolume = value;
            AudioManager.SetSoundVolume(value); 
        }
    }
    private void SaveAudioConfig()
    {
        var audioHandle = new AudioHandle(s_EnableAudio, s_AllAudioVolume, s_MusicVolume, s_SoundVolume);
        JsonManager.SaveData<AudioHandle>(audioHandle, m_AudioConfigPath);
        AssetDatabase.Refresh();
        Log.Editor("Save Succesful !");
    }
    private void LoadAudioConfig()
    {
        var audioConfig = JsonManager.LoadData<AudioHandle>(m_AudioConfigPath);

        if(audioConfig==null)
            return;

        s_EnableAudio = audioConfig.EnableAudio;
        s_AllAudioVolume = audioConfig.GlobalVolume;
        s_MusicVolume = audioConfig.BGMVolume;
        s_SoundVolume = audioConfig.SoundVolume;
    }
    private void DrawAudio()
    {
        using(new EditorGUILayout.VerticalScope())
        {
            using (var isMute = new EditorGUILayout.ToggleGroupScope("Mute", s_EnableAudio))
            {
                GUILayout.Space(10);
                using(new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Global Volume: ", EditorStyles.boldLabel,GUILayout.MaxWidth(120));
                    s_AllAudioVolume = GUILayout.HorizontalSlider(s_AllAudioVolume, 0, 1 , GUILayout.Width(200));
                    GUILayout.Space(10);
                    s_AllAudioVolume = EditorGUILayout.FloatField(s_AllAudioVolume*100, GUILayout.Width(70))/100f;
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10);
                using(new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("BGM Volume: ", EditorStyles.boldLabel,GUILayout.MaxWidth(120));
                    s_MusicVolume = GUILayout.HorizontalSlider(s_MusicVolume, 0, 1 , GUILayout.Width(200));
                    GUILayout.Space(10);
                    s_MusicVolume = EditorGUILayout.FloatField(s_MusicVolume*100, GUILayout.Width(70))/100f;
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10);
                using(new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Sound Volume: ", EditorStyles.boldLabel,GUILayout.MaxWidth(120));
                    s_SoundVolume = GUILayout.HorizontalSlider(s_SoundVolume, 0, 1 , GUILayout.Width(200));
                    GUILayout.Space(10);
                    s_SoundVolume = EditorGUILayout.FloatField(s_SoundVolume*100, GUILayout.Width(70))/100f;
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply"))
                {
                    SaveAudioConfig();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
