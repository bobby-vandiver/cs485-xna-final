using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FinalProject
{
    public class Audio : Microsoft.Xna.Framework.GameComponent
    {
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;        
        Cue backgroundCue;


        AudioEngine planetAudioEngine;
        WaveBank planetWaveBank;
        SoundBank planetSoundBank;
        Cue planetBackgroundCue;


        public Audio(Game game)
            : base(game)
        {
            // Load all the audio data
            audioEngine = new AudioEngine(@"Content\Audio\SnakeAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");

            planetAudioEngine = new AudioEngine(@"Content\Audio\AlienPlanetAudio.xgs");
            planetWaveBank = new WaveBank(audioEngine, @"Content\Audio\planetWaveBank.xwb");
            planetSoundBank = new SoundBank(audioEngine, @"Content\Audio\planetSoundBank.xsb");


        }




        public void PlayCue(string cueName)
        {
            soundBank.PlayCue(cueName);
        }

        public void PlayBackgroundMusic(string cueName)
        {
            backgroundCue = soundBank.GetCue(cueName);
            backgroundCue.Play();
        }

        public void StopBackgroundMusic()
        {
            if(backgroundCue != null)
                backgroundCue.Stop(AudioStopOptions.Immediate);
        }

        public override void Update(GameTime gameTime)
        {
            audioEngine.Update();
            planetAudioEngine.Update();
            base.Update(gameTime);
        }





        public void planetPlayCue(string cueName)
        {
            planetSoundBank.PlayCue(cueName);
        }

      



    }
}
