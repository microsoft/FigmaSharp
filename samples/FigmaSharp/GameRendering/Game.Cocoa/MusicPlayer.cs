using Foundation;

namespace Game.Cocoa
{
	public class MusicPlayer : IMusicPlayer
	{
		AVFoundation.AVAudioPlayer aVAudioPlayer;

		public MusicPlayer(string name, string type)
		{
			var backgroundMusicPath = new NSUrl(NSBundle.MainBundle.PathForResource(name, type));
			aVAudioPlayer = AVFoundation.AVAudioPlayer.FromUrl(backgroundMusicPath, out NSError error);
		}

		public void Play(int loops = 0)
		{
			aVAudioPlayer.NumberOfLoops = loops;
			aVAudioPlayer.Play();
		}
	}
}
