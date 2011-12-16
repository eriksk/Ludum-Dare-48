using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Gex.Audio
{
    public class AudioManager
    {
        private AudioEngine engine;
        private SoundBank sounds;
        private WaveBank waves;
        private Dictionary<string, Cue> songs;

        public AudioManager()
        {
            songs = new Dictionary<string, Cue>();
        }

        public void LoadContent(string path)
        {
            engine = new AudioEngine(path + "sounds.xgs");
            sounds = new SoundBank(engine, path + "Sound Bank.xsb");
            waves = new WaveBank(engine, path + "Wave Bank.xwb");
        }

        public void Clear()
        {
            foreach (KeyValuePair<string, Cue> c  in songs)
            {
                c.Value.Stop(AudioStopOptions.Immediate);
            }
            songs.Clear();
        }

        public void PlaySound(string name)
        {
            sounds.PlayCue(name);
        }
        public void PlaySong(string name)
        {
            Cue c = sounds.GetCue(name);
            songs.Add(name, c);
            c.Play();
        }        
        public void PauseSong(string name)
        {
            songs[name].Pause();
        }
        public void ResumeSong(string name)
        {
            songs[name].Resume();
        }
    }
}
