using System;
using AppKit;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Services;
using FigmaSharp.Cocoa;
using System.Collections.Generic;
using System.Linq;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace Game.Cocoa
{
	enum PlayerMovement
	{
		Left, Up, Right, Down
	}

	public class GameWindow : Window
    {
        const int WalkModifier = 13;
      
        IImageView[] spikesTiles;
		IImageView[] wallTiles;

        List<IImageView> gemsTiles;
        List<IImageView> heartTiles;
        ILabel pointsLabel;
		IImageView playerTile;
        int points = 0;

		Point startingPoint;
		IMusicPlayer backgroundMusic;
		IMusicPlayer coinSound;
		IMusicPlayer gameOverSound;

        public GameWindow(Rectangle rect) : base(rect)
        {
			Resizable = false;

            //we get the default basic view converters from the current loaded toolkit
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();

            //in this case we want use a remote file provider (figma url from our document)
            var fileProvider = new FigmaRemoteFileProvider();
            fileProvider.Load("Jv8kwhoRsrmtJDsSHcTgWGYu");

            //we initialize our renderer service, this uses all the converters passed
            //and generate a collection of NodesProcessed which is basically contains <FigmaModel, IView, FigmaParentModel>
            var rendererService = new FigmaViewRendererService(fileProvider, converters);

			//play background music
			backgroundMusic = new MusicPlayer ("Background", "mp3");
			backgroundMusic.Play(-1);

			coinSound = new MusicPlayer("Coin", "mp3");
			gameOverSound = new MusicPlayer("GameOver", "mp3");

			//we want load the entire level 1
			IView view = rendererService.RenderByName <IView>("Level1");
			Content = view;

            playerTile = rendererService.FindViewStartsWith<IImageView>("Player");

            startingPoint = playerTile.Allocation.Origin;

            pointsLabel = rendererService.FindViewByName<ILabel>("Points");
            gemsTiles = rendererService.FindViewsStartsWith<IImageView>("Gem")
                .ToList ();
            wallTiles = rendererService.FindViewsStartsWith<IImageView>("Tile")
                .ToArray();
			spikesTiles = rendererService.FindViewsStartsWith<IImageView>("Spikes")
				.ToArray();
			heartTiles = rendererService.FindViewsStartsWith<IImageView>("Heart")
				.OrderBy(s => s.Allocation.X)
				.ToList();
			WorldUpdate();
		}

		void MovePlayerToPoint(Point point)
		{
			if (IsPlayerPositionCollidingWithAnyWall (point))
				return;
			playerTile.SetPosition(point.X, point.Y);
			WorldUpdate();
		}

		bool IsPlayerPositionCollidingWithAnyWall (Point point)
		{
			var playerPosition = new Rectangle(point, playerTile.Size);
			foreach (var gem in wallTiles)
			{
				if (gem.Allocation.IntersectsWith(playerPosition))
					return true;
			}
			return false;
		}

		void WorldUpdate()
		{
			var playerPosition = playerTile.Allocation;
			//if user is in a 
			foreach (var spike in spikesTiles)
			{
				if (spike.Allocation.IntersectsWith(playerPosition))
				{
					PerformPlayerDied();
					return;
				}
			}

			foreach (var gem in gemsTiles)
			{
				if (gem.Allocation.IntersectsWith(playerTile.Allocation))
				{
					gemsTiles.Remove(gem);
					gem.Parent.RemoveChild(gem);
					points++;
					coinSound.Play();
					break;
				}
			}
			pointsLabel.Text = points.ToString();
		}

		void PerformPlayerDied()
		{
			playerTile.SetPosition(startingPoint);
			var lastLive = heartTiles.FirstOrDefault();
			if (lastLive != null)
			{
				heartTiles.Remove(lastLive);
				lastLive.Parent.RemoveChild(lastLive);
			}
			gameOverSound.Play();
		}

		void MovePlayer(PlayerMovement playerMovement)
		{
			Point position = playerTile.Allocation.Origin;
			switch (playerMovement)
			{
				case PlayerMovement.Left:
					position.X -= WalkModifier;
					break;
				case PlayerMovement.Right:
					position.X += WalkModifier;
					break;
				case PlayerMovement.Up:
					position.Y -= WalkModifier;
					break;
				default:
					// PlayerMovement.Bottom
					position.Y += WalkModifier;
					break;
			}
			MovePlayerToPoint(position);
		}

		protected override void OnKeyDownPressed(object sender, Key args)
		{
			switch (args)
			{
				case Key.LeftArrow:
					MovePlayer(PlayerMovement.Left);
					break;
				case Key.RightArrow:
					MovePlayer(PlayerMovement.Right);
					break;
				case Key.UpArrow:
					MovePlayer(PlayerMovement.Up);
					break;
				case Key.DownArrow:
					MovePlayer(PlayerMovement.Down);
					break;
			}
		}
	}
}
