using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;


namespace tg_bot_controler
{
    internal class SoundPlayer
    {
        WaveOutEvent waveOut;
        Mp3FileReader reader;
        string pathSound;

        public void PlaySound(string path)
        {
            pathSound = path;
            reader = new Mp3FileReader(path);
            waveOut = new WaveOutEvent(); // or WaveOutEvent()
            waveOut.Init(reader);            
            waveOut.Play();
            waveOut.PlaybackStopped += OnPlaybackStopped;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            Console.WriteLine("Воспроизведение завершено!");
            
            waveOut.Dispose();
            reader.Dispose();

            waveOut = null;
            reader = null;
            File.Delete(pathSound);
        }

        public void SetOutputDevice()
        {
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Console.WriteLine($"{n}: {caps.ProductName}");
            }
        }
    }
}
