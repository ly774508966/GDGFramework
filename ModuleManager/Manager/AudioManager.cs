using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GDG.ECS;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.ModuleManager
{
    public class AudioManager : LazySingleton<AudioManager>
    {
        public AudioManager()
        {
#if DEBUG
            UserFileManager.BuildFolder_Async("Audio", $"{Application.dataPath}/Resources");
            UserFileManager.BuildFolder_Async("Music", $"{Application.dataPath}/Resources/Audio");
            UserFileManager.BuildFolder_Async("Sound", $"{Application.dataPath}/Resources/Audio");
#endif
            World.monoWorld.AddOrRemoveListener(Update,"Update");
            EventManager.Instance.AddActionListener<AudioSource>("StopSound", StopSoundWhenOver);
        }
        private void Update()
        {
            for (int i = soundlist.Count - 1; i >= 0; --i)
            {
                EventManager.Instance.ActionTrigger<AudioSource>("StopSound", soundlist[i]);
            }
        }
        private GameObject musicplayer = null;
        public static bool IsMute = true;
        public static float GlobalVolume = 1;
        public static float MusicVolume = 1;
        public static float SoundVolume = 1;
        private static AudioSource music;
        private static List<AudioSource> soundlist = new List<AudioSource>();
        #region 音乐管理
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="resourcename">Resources文件夹下路径</param>
        /// <param name="isLoop">是否循环，默认true</param>
        /// <param name="volume">音量大小(range:0-1)，默认1</param>
        public void PlayMusic(string resourcename, bool isLoop = true, float volume = 1)
        {
            if (musicplayer == null)
            {
                musicplayer = new GameObject("MusicPlayer");
                musicplayer.transform.parent = GameObject.Find("MonoController").transform;
            }
            if (music == null)
                music = musicplayer.AddComponent<AudioSource>();
            ResourcesManager.Instance.LoadResourceAsync<AudioClip>($"Audio/Music/{resourcename}", (clip) =>
            {
                music.clip = clip;
                music.loop = isLoop;
                music.volume = volume;
                if(!IsMute)
                music.Play();
            });
        }
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="bundlename">AB包名称</param>
        /// <param name="assetname">asset名称</param>
        /// <param name="isLoop">是否循环，默认true</param>
        /// <param name="volume">音量大小(range:0-1)，默认1</param>
        public void PlayMusic(string bundlename, string assetname, bool isLoop = true, float volume = 1)
        {
            if (musicplayer == null)
            {
                musicplayer = new GameObject("MusicPlayer");
                musicplayer.transform.parent = GameObject.Find("MonoController").transform;
            }
            if (music == null)
                music = musicplayer.AddComponent<AudioSource>();
            AssetManager.Instance.LoadAssetAsync<AudioClip>(bundlename, assetname, (clip) =>
            {
                music.clip = clip;
                music.loop = isLoop;
                music.volume = volume;
                if(!IsMute)
                music.Play();
            });
        }
        /// <summary>
        /// 音乐暂停
        /// </summary>
        public static void PauseMusic() => music?.Pause();
        /// <summary>
        /// 音乐停止
        /// </summary>
        public static void StopMusic() => music?.Stop();
        /// <summary>
        /// 设置音量大小
        /// </summary>
        public static void SetMusicVolume(float value)
        {
            if (music != null)
                music.volume = value;
        }
        #endregion
        #region 音效管理
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="resourcename">Resources文件夹下路径</param>
        /// <param name="callback">回调函数</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="volume">音量大小(range:0-1)，默认1</param>
        public void PlaySound(string resourcename, bool isLoop = false, float volume = 1, UnityAction<AudioSource> callback = null)
        {
            if (musicplayer == null)
            {
                musicplayer = new GameObject("MusicPlayer");
                musicplayer.transform.parent = GameObject.Find("MonoController").transform;
            }
            //从对象池中加载音频切片
            AssetPool.Instance.Pop<AudioClip>($"Audio/Sound/{resourcename}", (clip) =>
            {
                AudioSource sound = musicplayer.AddComponent<AudioSource>();

                sound.clip = clip;
                sound.loop = isLoop;
                sound.volume = volume;
                if(!IsMute)
                sound.Play();
                soundlist.Add(sound);
                if (callback != null)
                    callback(sound);
            });
        }
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="bundlename">AB包名称</param>
        /// <param name="assetname">asset名称</param>
        /// <param name="callback">回调函数</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="volume">音量大小(range:0-1)，默认1</param>
        public void PlaySound(string bundlename, string assetname,  bool isLoop = false, float volume = 1,UnityAction<AudioSource> callback = null)
        {
            if (musicplayer == null)
            {
                musicplayer = new GameObject("MusicPlayer");
                musicplayer.transform.parent = GameObject.Find("MonoController").transform;
            }
            AssetPool.Instance.Pop<AudioClip>(bundlename, assetname, (clip) =>
            {
                AudioSource sound = musicplayer.AddComponent<AudioSource>();
                sound.clip = clip;
                sound.loop = isLoop;
                sound.volume = volume;
                
                if(!IsMute)
                sound.Play();
                soundlist.Add(sound);
                if (callback != null)
                    callback(sound);

            });
        }
        /// <summary>
        /// 停止播放所以音效
        /// </summary>
        public static void StopSound()
        {
            foreach (var sound in soundlist)
            {
                Object.Destroy(sound);
            }
        }
        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="sound"></param>
        public void StopSound(AudioSource sound)
        {
            Object.Destroy(sound);
            soundlist.Remove(sound);
        }
        private void StopSoundWhenOver(AudioSource sound)
        {
            if (!sound.isPlaying)
            {
                soundlist.Remove(sound);
                AssetPool.Instance.Push<AudioClip>(sound.clip.name, sound.clip, () => { Object.Destroy(sound); });
            }
        }
        /// <summary>
        /// 设置音效的大小
        /// </summary>
        /// <param name="value"></param>
        public static void SetSoundVolume(float value)
        {
            foreach (var sound in soundlist)
            {
                if (sound != null)
                    sound.volume = value;
            }
        }


        #endregion
        #region 3D音效管理
        public void Play3DSound(string resourcespath, GameObject gameobject, bool isLoop = false, float volume = 2f,UnityAction<AudioSource> callback = null)
        {
            string pattern = @"[^\/]+$";
            string file = Regex.Match(resourcespath, pattern).ToString();

            //从对象池中加载音频切片
            AssetPool.Instance.Pop<AudioClip>($"Audio/Sound/{resourcespath}", (clip) =>
            {
                AudioSource sound = null;

                sound = gameobject.AddComponent<AudioSource>();

                sound.clip = clip;
                sound.loop = isLoop;
                sound.volume = volume;
                sound.spatialBlend = 1;
                sound.minDistance = 1;
                sound.maxDistance = 45;
                sound.volume = volume;
                if(!IsMute)
                sound.Play();
                soundlist.Add(sound);
                if (callback != null)
                    callback(sound);
            }, file);
        }
        public void Play3DSound(string bundlename, string assetname, GameObject gameobject, bool isLoop = false, float volume = 2f,UnityAction<AudioSource> callback = null)
        {
            AssetPool.Instance.Pop<AudioClip>(bundlename, assetname, (clip) =>
            {
                AudioSource[] sources = gameobject.GetComponents<AudioSource>();
                AudioSource sound = null;
                bool isExistClip = false;
                if (sources != null)
                    foreach (AudioSource source in sources)
                    {
                        if (source.clip.name == clip.name)
                        {
                            isExistClip = true;
                            sound = source;
                        }
                    }

                if (isExistClip)
                    sound.enabled = true;
                else
                {
                    sound = gameobject.AddComponent<AudioSource>();

                    sound.clip = clip;
                    sound.loop = isLoop;
                    sound.volume = volume;
                    sound.spatialBlend = 1;
                    sound.minDistance = 1;
                    sound.maxDistance = 45;
                    sound.volume = volume;
                    if(!IsMute)
                    sound.Play();
                    soundlist.Add(sound);
                }
                if (callback != null)
                    callback(sound);
            });
        }
        #endregion
        #region 总声音管理
        public static void StopAllAudio()
        {
            StopMusic();
            StopSound();
        }
        public static void SetAllAudioVolume(float value)
        {
            SetSoundVolume(value);
            SetMusicVolume(value);
        }
        #endregion
    }
}