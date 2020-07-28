﻿// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;

using FigmaSharp;
using FigmaSharp.Services;
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

            //in this case we want use a remote file provider (figma url from our document)
            var fileProvider = new RemoteNodeProvider();
            fileProvider.Load("Jv8kwhoRsrmtJDsSHcTgWGYu");

			//we initialize our renderer service, this uses all the converters passed
			//and generate a collection of NodesProcessed which is basically contains <FigmaModel, IView, FigmaParentModel>
			
            var rendererService = new ViewRenderService(fileProvider);

			//play background music
			backgroundMusic = new MusicPlayer ("Background", "mp3");
			backgroundMusic.Play(-1);

			coinSound = new MusicPlayer("Coin", "mp3");
			gameOverSound = new MusicPlayer("GameOver", "mp3");

			//we want load the entire level 1
			rendererService.RenderInWindow (this, "Level1");

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
