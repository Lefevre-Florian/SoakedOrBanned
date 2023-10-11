using Godot;
using System;
using Com.IsartDigital.Utils.Events;

//Author : VERDIER Thomas 
namespace Com.IsartDigital.Sokoban.UI.LevelSelector {

	public class PopUp : Control
	{
		[Export] private NodePath ButtonOpenPath = default;

		[Export] private float MenuSize = 0.35f;
		[Export] private float SpeedLerp = 0.2f;

		private bool IsPopedUp = false;
		
		private Button ButtonOpen;
		
		private Vector2 AnchorUp;
		private Vector2 DownAnchor;
		private Vector2 TargetAnchor;

		public override void _Ready()
		{

			ButtonOpen = GetNode<Button>(ButtonOpenPath);
			DownAnchor = new Vector2(1, 1 + MenuSize);
			AnchorUp = new Vector2(1-MenuSize, 1); 
			TargetAnchor = DownAnchor;
			ButtonOpen.Connect(EventButton.PRESSED, this, nameof(OpenThisPopUp));
			Connect(EventNode.TREE_EXITING, this, nameof(Destructor));
			ButtonOpen.Text = "^";
		}

        public override void _Process(float delta)
        {
			AnchorTop = Mathf.Lerp(AnchorTop, TargetAnchor.x, SpeedLerp);
			AnchorBottom = Mathf.Lerp(AnchorBottom, TargetAnchor.y, SpeedLerp);
			base._Process(delta);
        }

		private void OpenThisPopUp()
        {
			if (!IsPopedUp)
			{
				ButtonOpen.Text = "X";
				TargetAnchor = AnchorUp;
			}
			else
			{
				ButtonOpen.Text = "^";
				TargetAnchor = DownAnchor;
			}
			IsPopedUp = !IsPopedUp; 
        }

		private void Destructor()
        {
			ButtonOpen.Disconnect(EventButton.PRESSED, this, nameof(OpenThisPopUp));
			Disconnect(EventNode.TREE_EXITING, this, nameof(Destructor));
        }

    }

}