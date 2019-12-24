using BS_Utils.Utilities;
using IPA;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace SoundEffectReplacer
{
    public class Plugin : IBeatSaberPlugin
    {
        readonly BS_Utils.Utilities.Config ConfigVariable = new BS_Utils.Utilities.Config("SoundEffectReplacer/SoundEffectReplacer");
        public string audioFolder = Environment.CurrentDirectory + "/UserData/SoundEffectReplacer";

        // private AudioClip _creditsAudioClip;

        // private CreditsController creditsController;

        // GameSoundManager -> _noteJumpAudioClips
        // GameSoundManager -> _noteWasCutAudioClip
        // VisualMetronome?
        // SaberSound?
        // FireworkItemController?


        public static NoteCutSoundEffectManager noteCutSoundEffectManager;
        public static NoteCutSoundEffect noteCutSoundEffect;
        public static BombCutSoundEffectManager bombCutSoundEffectManager;
        public static BasicUIAudioManager basicUIAudioManager;

        public static AudioClip[] _longCutEffectsAudioClips;
        public static AudioClip[] _shortCutEffectsAudioClips;
        public static AudioClip[] _badCutSoundEffectAudioClips;
        public static AudioClip[] _bombExplosionAudioClips;
        public static AudioClip[] _clickSounds;

        public void Init(IPALogger logger)
        {
            Logger.log = logger;
            if (!Directory.Exists(audioFolder + "/badcut"))
                Directory.CreateDirectory(audioFolder + "/badcut");
            if (!Directory.Exists(audioFolder + "/shortcut"))
                Directory.CreateDirectory(audioFolder + "/shortcut");
            if (!Directory.Exists(audioFolder + "/longcut"))
                Directory.CreateDirectory(audioFolder + "/longcut");
            if (!Directory.Exists(audioFolder + "/bomb"))
                Directory.CreateDirectory(audioFolder + "/bomb");
            if (!Directory.Exists(audioFolder + "/click"))
                Directory.CreateDirectory(audioFolder + "/click");
        }

        public void OnApplicationStart() { }

        public void OnApplicationQuit() { }

        public void OnFixedUpdate() { }

        public void OnUpdate() { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            Logger.log.Debug(nextScene.name);
            SharedCoroutineStarter.instance.StartCoroutine(LoadAudio());
            if (nextScene.name == "MenuViewControllers")
            {
                basicUIAudioManager = Resources.FindObjectsOfTypeAll<BasicUIAudioManager>().FirstOrDefault();
                if (_clickSounds != null)
                {
                    ReflectionUtil.SetField(basicUIAudioManager, "_clickSounds", _clickSounds);
                }
            }
            if (nextScene.name == "GameCore")
            {
                noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault();
                noteCutSoundEffect = Resources.FindObjectsOfTypeAll<NoteCutSoundEffect>().FirstOrDefault();
                bombCutSoundEffectManager = Resources.FindObjectsOfTypeAll<BombCutSoundEffectManager>().FirstOrDefault();
                if (_longCutEffectsAudioClips != null)
                {
                    ReflectionUtil.SetField(noteCutSoundEffectManager, "_longCutEffectsAudioClips", _longCutEffectsAudioClips);
                }
                if (_shortCutEffectsAudioClips != null)
                {
                    ReflectionUtil.SetField(noteCutSoundEffectManager, "_shortCutEffectsAudioClips", _shortCutEffectsAudioClips);
                }
                if (_badCutSoundEffectAudioClips != null)
                {
                    ReflectionUtil.SetField(noteCutSoundEffect, "_badCutSoundEffectAudioClips", _badCutSoundEffectAudioClips);
                }
                if (_bombExplosionAudioClips != null)
                {
                    ReflectionUtil.SetField(bombCutSoundEffectManager, "_bombExplosionAudioClips", _bombExplosionAudioClips);
                }
            }
        }

        public IEnumerator LoadAudio()
        {
            if (_longCutEffectsAudioClips == null)
            {
                if (Directory.Exists(audioFolder + "/longcut") && Directory.GetFiles(audioFolder + "/longcut").Length > 0)
                {
                    string[] audioFiles = Directory.GetFiles(audioFolder + "/longcut");
                    _longCutEffectsAudioClips = new AudioClip[audioFiles.Length];
                    int index = 0;
                    foreach (string filePath in audioFiles)
                    {
                        string file = "file:///" + filePath;
                        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.OGGVORBIS);
                        AudioClip audioFile = null;
                        yield return www.SendWebRequest();
                        audioFile = DownloadHandlerAudioClip.GetContent(www);
                        if (audioFile != null || audioFile.frequency == 0)
                        {
                            _longCutEffectsAudioClips[index] = audioFile;
                            Logger.log.Debug(audioFile.frequency + " | Audio loaded: " + file);
                            index++;
                        }
                        else
                        {
                            Logger.log.Debug(audioFile.frequency + " | Audio didn't load: " + file);
                        }
                    }
                }
                if (_shortCutEffectsAudioClips == null)
                {
                    if (Directory.Exists(audioFolder + "/shortcut") && Directory.GetFiles(audioFolder + "/shortcut").Length > 0)
                    {
                        string[] audioFiles = Directory.GetFiles(audioFolder + "/shortcut");
                        _shortCutEffectsAudioClips = new AudioClip[audioFiles.Length];
                        int index = 0;
                        foreach (string filePath in audioFiles)
                        {
                            string file = "file:///" + filePath;
                            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.OGGVORBIS);
                            AudioClip audioFile = null;
                            yield return www.SendWebRequest();
                            audioFile = DownloadHandlerAudioClip.GetContent(www);
                            if (audioFile != null || audioFile.frequency == 0)
                            {
                                _shortCutEffectsAudioClips[index] = audioFile;
                                Logger.log.Debug(audioFile.frequency + " | Audio loaded: " + file);
                                index++;
                            }
                            else
                            {
                                Logger.log.Debug(audioFile.frequency + " | Audio didn't load: " + file);
                            }
                        }
                    }
                }
                if (_badCutSoundEffectAudioClips == null)
                {
                    if (Directory.Exists(audioFolder + "/badcut") && Directory.GetFiles(audioFolder + "/badcut").Length > 0)
                    {
                        string[] audioFiles = Directory.GetFiles(audioFolder + "/badcut");
                        _badCutSoundEffectAudioClips = new AudioClip[audioFiles.Length];
                        int index = 0;
                        foreach (string filePath in audioFiles)
                        {
                            string file = "file:///" + filePath;
                            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.OGGVORBIS);
                            AudioClip audioFile = null;
                            yield return www.SendWebRequest();
                            audioFile = DownloadHandlerAudioClip.GetContent(www);
                            if (audioFile != null || audioFile.frequency == 0)
                            {
                                _badCutSoundEffectAudioClips[index] = audioFile;
                                Logger.log.Debug(audioFile.frequency + " | Audio loaded: " + file);
                                index++;
                            }
                            else
                            {
                                Logger.log.Debug(audioFile.frequency + " | Audio didn't load: " + file);
                            }
                        }
                    }
                }
                if (_bombExplosionAudioClips == null)
                {
                    if (Directory.Exists(audioFolder + "/bomb") && Directory.GetFiles(audioFolder + "/bomb").Length > 0)
                    {
                        string[] audioFiles = Directory.GetFiles(audioFolder + "/bomb");
                        _bombExplosionAudioClips = new AudioClip[audioFiles.Length];
                        int index = 0;
                        foreach (string filePath in audioFiles)
                        {
                            string file = "file:///" + filePath;
                            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.OGGVORBIS);
                            AudioClip audioFile = null;
                            yield return www.SendWebRequest();
                            audioFile = DownloadHandlerAudioClip.GetContent(www);
                            if (audioFile != null || audioFile.frequency == 0)
                            {
                                _bombExplosionAudioClips[index] = audioFile;
                                Logger.log.Debug(audioFile.frequency + " | Audio loaded: " + file);
                                index++;
                            }
                            else
                            {
                                Logger.log.Debug(audioFile.frequency + " | Audio didn't load: " + file);
                            }
                        }
                    }
                }
                if (_clickSounds == null)
                {
                    if (Directory.Exists(audioFolder + "/click") && Directory.GetFiles(audioFolder + "/click").Length > 0)
                    {
                        string[] audioFiles = Directory.GetFiles(audioFolder + "/click");
                        _clickSounds = new AudioClip[audioFiles.Length];
                        int index = 0;
                        foreach (string filePath in audioFiles)
                        {
                            string file = "file:///" + filePath;
                            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.OGGVORBIS);
                            AudioClip audioFile = null;
                            yield return www.SendWebRequest();
                            audioFile = DownloadHandlerAudioClip.GetContent(www);
                            if (audioFile != null || audioFile.frequency == 0)
                            {
                                _clickSounds[index] = audioFile;
                                Logger.log.Debug(audioFile.frequency + " | Audio loaded: " + file);
                                index++;
                            }
                            else
                            {
                                Logger.log.Debug(audioFile.frequency + " | Audio didn't load: " + file);
                            }
                        }
                    }
                }
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }

        public void OnSceneUnloaded(Scene scene) { }
    }
}
