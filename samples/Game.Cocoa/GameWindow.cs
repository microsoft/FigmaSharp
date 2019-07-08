using System;
using AppKit;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Services;
using FigmaSharp.Cocoa;
using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace Game.Cocoa
{
    public class GameWindow : NSWindow
    {
        const int WalkModifier = 13;
      
        NSImageView[] spikesTiles;
        NSImageView[] wallTiles;

        List<NSImageView> gemsTiles;
        List<NSImageView> heartTiles;
        NSTextField pointsLabel;
        NSView playerTile;
        int points = 0;

        #region Player Movement

        public override void KeyDown(NSEvent theEvent)
        {
            if (theEvent.KeyCode == (ushort)NSKey.LeftArrow)
            {
                MovePlayer(new CGPoint(playerTile.Frame.X - WalkModifier, playerTile.Frame.Y));
                return;
            }

            if (theEvent.KeyCode == (ushort)NSKey.RightArrow)
            {
                MovePlayer(new CGPoint(playerTile.Frame.X + WalkModifier, playerTile.Frame.Y));
                return;
            }

            if (theEvent.KeyCode == (ushort)NSKey.UpArrow)
            {
                MovePlayer(new CGPoint(playerTile.Frame.X, playerTile.Frame.Y + WalkModifier));
                return;
            }

            if (theEvent.KeyCode == (ushort)NSKey.DownArrow)
            {
                MovePlayer(new CGPoint(playerTile.Frame.X, playerTile.Frame.Y - WalkModifier));
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
                if (gem.Frame.IntersectsWith (playerTile.Frame))
                {
                    gemsTiles.Remove(gem);
                    gem.RemoveFromSuperview();
                    points++;
                    coinSound.Play();
                    break;
                }
            }
            pointsLabel.StringValue = points.ToString ();
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

        public GameWindow(CGRect rect) : base(rect, NSWindowStyle.Titled | NSWindowStyle.Closable, NSBackingStore.Buffered, false)
        {
            //we get the default basic view converters from the current loaded toolkit
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();

            //in this case we want use a remote file provider (figma url from our document)
            var fileProvider = new FigmaRemoteFileProvider();
            fileProvider.Load("Jv8kwhoRsrmtJDsSHcTgWGYu");

            //we initialize our renderer service, this uses all the converters passed
            //and generate a collection of NodesProcessed which is basically contains <FigmaModel, IViewWrapper, FigmaParentModel>
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
            IViewWrapper view = rendererService.RenderByName<IViewWrapper>("Level1");

            ContentView = view.NativeObject as NSView;

            playerTile = rendererService.FindNativeViewByName<NSImageView>("Player");

            startingPoint = playerTile.Frame.Location;

            pointsLabel = rendererService.FindNativeViewByName<NSTextField>("Points");

            gemsTiles = rendererService.FindNativeViewsByName<NSImageView>("Gem")
                .ToList ();

            wallTiles = rendererService.FindNativeViewsStartsWith<NSImageView>("Tile")
                .ToArray();

            spikesTiles = rendererService.FindNativeViewsStartsWith<NSImageView>("Spikes")
                .ToArray();

            heartTiles = rendererService.FindNativeViewsStartsWith<NSImageView>("Heart")
                .OrderBy(s => s.Frame.X)
                .ToList();

            Refresh();
        }
    }
}
