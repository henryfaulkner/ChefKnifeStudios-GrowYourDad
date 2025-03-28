using Godot;
using System;

public partial class UpgradeMenuOption : Control
{
	[Signal]
	public delegate void PressedEventHandler();

	private static readonly StringName PANEL_STYLEBOX_NAME = new StringName("panel");
	private static readonly StringName NORMAL_PANEL_STYLE = new StringName("res://Pages/UpgradeScene/Inactive_UpgradeMenuOption.tres");
	private static readonly StringName HOVER_PANEL_STYLE = new StringName("res://Pages/UpgradeScene/Active_UpgradeMenuOption.tres");

	[ExportGroup("Nodes")]
	[Export]
	Panel Panel { get; set; } = null!;
	[Export]
	Godot.TextureButton TextureBtn { get; set; } = null!;

	[ExportGroup("Textures")]
	[Export]
	private Texture2D NormalTexture { get; set; } = null!;
	[Export]
	private Texture2D HoverTexture { get; set; } = null!;
	
	private StyleBoxFlat ActivePagePanelOptionStyle { get; set; } = null!;
	private StyleBoxFlat InactivePagePanelOptionStyle { get; set; } = null!;
	
	LoggerService _logger = null!;

	public UpgradeMenuOption() 
	{
		ActivePagePanelOptionStyle = GD.Load<StyleBoxFlat>(HOVER_PANEL_STYLE);
		InactivePagePanelOptionStyle = GD.Load<StyleBoxFlat>(NORMAL_PANEL_STYLE);
	}

	public override void _Ready()
	{
		if (NormalTexture != null) TextureBtn.TextureNormal = NormalTexture;
		if (HoverTexture != null) TextureBtn.TextureHover = HoverTexture;
		
		TextureBtn.MouseEntered += HandleMouseEntered;
		TextureBtn.MouseExited += HandleMouseExited;
		TextureBtn.Pressed += HandlePressed;
		
		_logger = GetNode<LoggerService>(Constants.SingletonNodes.LoggerService);
	}

	public override void _ExitTree()
	{
		if (TextureBtn != null)
		{
			TextureBtn.MouseEntered -= HandleMouseEntered;
			TextureBtn.MouseExited -= HandleMouseExited;
			TextureBtn.Pressed -= HandlePressed;
		}
	}

	public void GrabFocus()
	{
		TextureBtn.GrabFocus();
		ApplyActivePagePanelOption(Panel);
	}

	public void LoseFocus()
	{
		ApplyInactivePagePanelOption(Panel);
	}

	void HandleMouseEntered()
	{
		_logger.LogDebug("Call HandleMouseEntered");
		ApplyActivePagePanelOption(Panel);
	}

	void HandleMouseExited()
	{
		_logger.LogDebug("Call HandleMouseExited");
		ApplyInactivePagePanelOption(Panel);
	}
	
	void HandlePressed()
	{
		_logger.LogDebug("Call HandlePressed");
		EmitSignal(SignalName.Pressed);
	}	

	void ApplyActivePagePanelOption(Panel panel)
	{
		panel.AddThemeStyleboxOverride(PANEL_STYLEBOX_NAME, ActivePagePanelOptionStyle);
	}

	void ApplyInactivePagePanelOption(Panel panel)
	{
		panel.AddThemeStyleboxOverride(PANEL_STYLEBOX_NAME, InactivePagePanelOptionStyle);
	}
}
