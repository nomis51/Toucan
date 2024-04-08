using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using Avalonia.Platform;
using Menagerie.Enums;
using Menagerie.Services.Abstractions;
using Serilog;

namespace Menagerie.Services;

public class AudioService : IAudioService
{
    #region Singleton

    private static readonly object LockInstance = new();
    private static AudioService _instance;

    public static AudioService Instance
    {
        get
        {
            lock (LockInstance)
            {
                _instance ??= new AudioService();
            }

            return _instance;
        }
    }

    #endregion

    #region Members

    private static readonly object AudioEffectsLock = new();
#pragma warning disable CA1416
    private readonly Dictionary<AudioEffect, SoundPlayer> _audioEffects = new();
#pragma warning restore CA1416

    private readonly Dictionary<AudioEffect, string> _audioEffectsNames = new()
    {
        [AudioEffect.Click] = "click",
        [AudioEffect.PlayerJoined] = "playerJoinedNotification",
        [AudioEffect.NewTrade] = "newTradeNotification",
    };

    #endregion

    #region Constructors

    private AudioService()
    {
    }

    #endregion

    #region Public methods

    public void PlayEffect(AudioEffect effect)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        if (!EnsureAudioEffectLoaded(effect)) return;

        _audioEffects[effect].Play();
    }

    #endregion

    #region Private methods

    private bool EnsureAudioEffectLoaded(AudioEffect effect)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

        try
        {
            if (_audioEffects.ContainsKey(effect)) return true;

            lock (AudioEffectsLock)
            {
                if (_audioEffects.ContainsKey(effect)) return true;
                if (!_audioEffectsNames.ContainsKey(effect)) return false;

                _audioEffects.Add(
                    effect,
                    new SoundPlayer(
                        File.Open(
                            $"./Assets/audio/{_audioEffectsNames[effect]}.wav",
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite
                        )
                    )
                );
                return true;
            }
        }
        catch (Exception e)
        {
            Log.Warning("Unable to load audio effect {Effect}: {Message}", effect, e.Message);
        }

        return false;
    }

    #endregion
}