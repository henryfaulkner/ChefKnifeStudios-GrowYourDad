using Godot;
using System;

public partial class PreActionScene : Node2D
{
	[Export]
	Area2D DoorArea { get; set; }

	static readonly StringName ACTION_LEVEL_PATH = new StringName("res://Pages/ActionScene/ActionLevel.tscn");
	readonly PackedScene _actionLevelScene;

	ILoggerService _logger;
	PauseMenuService _pauseMenuService;

	public PreActionScene()
	{
		_actionLevelScene = (PackedScene)ResourceLoader.Load(ACTION_LEVEL_PATH);
	}

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);

		_pauseMenuService.EmitCloseMenu();
		
		DoorArea.AreaExited += HandleDoorAreaExited;
	}
	
	public override void _ExitTree()
	{
		if (DoorArea != null)
		{
			DoorArea.AreaExited -= HandleDoorAreaExited;
		}
	}

	void HandleDoorAreaExited(Area2D target)
	{ 
		// Use call_deferred to safely change the scene
		CallDeferred(nameof(ChangeToActionLevel));
	}

	void ChangeToActionLevel()
	{
		GetTree().ChangeSceneToPacked(_actionLevelScene);
	}
}
