using System;
using AppKit;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Services;
using FigmaSharp.Cocoa;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using LiteForms;
using LiteForms.Cocoa;

namespace Game.Cocoa
{
    public class GameWindow : Window
    {
        const int WalkModifier = 13;
      
        ImageView[] spikesTiles;
		ImageView[] wallTiles;

        List<ImageView> gemsTiles;
        List<ImageView> heartTiles;
        Label pointsLabel;
		ImageView playerTile;
        int points = 0;

        #region Player Movement

        public override void KeyDown(NSEvent theEvent)
        {
            if (theEvent.KeyCode == (ushort)NSKey.LeftArrow)
            {
				var frame = playerTile.Allocation;
				MovePlayer(new CGPoint(frame.X - WalkModifier, frame.Y));
                return;
            }

            if (theEvent.KeyCode == (ushort)NSKey.RightArrow)
            {
				var frame = playerTile.Allocation;
				MovePlayer(new CGPoint(frame.X + WalkModifier, frame.Y));
                return;
            }

            if (theEvent.KeyCode == (ushort)NSKey.UpArrow)
            {
				var frame = playerTile.Allocation;
				MovePlayer(new CGPoint(frame.X, frame.Y + WalkModifier));
                return;
            }

            if (theEvent.KeyCode == (ushort)NSKey.DownArrow)
            {
				var frame = playerTile.Allocation;
				MovePlayer(new CGPoint(frame.X, frame.Y - WalkModifier));
                return;
            }

            base.KeyDown(theEvent);
        }

        #endregion

        void MovePlayer (CGPoint point)
        {
            if (PlayerPositionCollidesWithWall (point))
                return;
            playerTile.SetFrameOrigin(point);

            Refresh();
        }

        bool PlayerPositionCollidesWithWall (CGPoint point)
        {
            var playerPosition = new CGRect(point, playerTile.Frame.Size);
            foreach (var gem in wallTiles)
            {
                if (gem.Frame.IntersectsWith(playerPosition))
                {
                    return true;
                }
            }
            return false;
        }

        void Refresh ()
        {
            //if user is in a 
            foreach (var spike in spikesTiles)
            {
                if (spike.Frame.IntersectsWith(playerTile.Frame))
                {
                    PlayerDied();
                    return;
                }
            }

            foreach (var gem in gemsTiles)
            {
                if (gem.Allocation.IntersectsWith (playerTile.Allocation))
                {
                    gemsTiles.Remove(gem);
                    gem.RemoveFromParent ();
                    points++;
                    coinSound.Play();
                    break;
                }
            }
            pointsLabel.Text StringValue = points.ToString ();
        }

        void PlayerDied ()
        {
            playerTile.SetFrameOrigin(startingPoint);
            var lastLive = heartTiles.FirstOrDefault();
            if (lastLive != null)
            {
                heartTiles.Remove(lastLive);
                lastLive.RemoveFromSuperview();
            }
            gameOverSound.Play();
        }

        CGPoint startingPoint;
        AVFoundation.AVAudioPlayer backgroundMusic;
        AVFoundation.AVAudioPlayer coinSound;
        AVFoundation.AVAudioPlayer gameOverSound;

        public GameWindow(Rectangle rect) : base(rect)
        {
            //we get the default basic view converters from the current loaded toolkit
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();

            //in this case we want use a remote file provider (figma url from our document)
            var fileProvider = new FigmaRemoteFileProvider();
            fileProvider.Load("Jv8kwhoRsrmtJDsSHcTgWGYu");

            //we initialize our renderer service, this uses all the converters passed
            //and generate a collection of NodesProcessed which is basically contains <FigmaModel, IView, FigmaParentModel>
            var rendererService = new FigmaViewRendererService(fileProvider, converters);

            //play background music
            var backgroundMusicPath = new NSUrl(NSBundle.MainBundle.PathForResource("Background", "mp3"));
            backgroundMusic = AVFoundation.AVAudioPlayer.FromUrl(backgroundMusicPath, out NSError error);
            backgroundMusic.NumberOfLoops = -1;
            backgroundMusic.Play();

            var gameOverSoundPath = new NSUrl(NSBundle.MainBundle.PathForResource("GameOver", "mp3"));
            gameOverSound = AVFoundation.AVAudioPlayer.FromUrl(gameOverSoundPath, out error);
         
            var coinMusicPath = new NSUrl(NSBundle.MainBundle.PathForResource("Coin", "mp3"));
            coinSound = AVFoundation.AVAudioPlayer.FromUrl(coinMusicPath, out error);

            //we want load the entire level 1
            IView view = rendererService.FindViewByName <IView>("Level1");
			Content = view;

            playerTile = rendererService.FindViewByName<ImageView>("Player");

            startingPoint = playerTile.Allocation.Location;

            pointsLabel = rendererService.FindViewByName<Label>("Points");

            gemsTiles = rendererService.FindViewsByName<ImageView>("Gem")
                .ToList ();

            wallTiles = rendererService.FindViewsByName<ImageView>("Tile")
                .ToArray();

            spikesTiles = rendererService.FindViewByName<ImageView>("Spikes")
                .ToArray();

            heartTiles = rendererService.FindViewByName<ImageView>("Heart")
                .OrderBy(s => s.Frame.X)
                .ToList();

            Refresh();
        }
    }
}
