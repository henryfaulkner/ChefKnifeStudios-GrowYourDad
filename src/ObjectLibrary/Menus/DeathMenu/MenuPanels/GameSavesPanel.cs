using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

public partial class GameSavesPanel : TextButtonMenuPanel
{
	[Export]
	TextButton[] GameSaveBtns { get; set; } = null!;
	[Export]
	TextButton BackBtn { get; set; } = null!;
	
	[Export]
	NewGameSavePopupMenu NewGameSavePopup { get; set; } = null!;

	public MenuBusiness MenuBusiness { get; set; } = null!;
	public override int Id => (int)Enumerations.DeathMenuPanels.GameSaves;

	const string OPTION_TEMPLATE = """
		[dropcap font_size=64 margins={0},0,0,0]
			{1}
		[/dropcap]
	""";
	const int MARGIN_INTERVAL = 18;

	List<string> Texts = null!;
	Action[] SelectHandlers = null!;

	IUnitOfWork _unitOfWork = null!;
	ICrawlStatsService _crawlStatsService = null!;

	List<GameSave> _gameSaveEntityList = null!;

	public override void _Ready()
	{
		_unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		
		NewGameSavePopup.Visible = false;
		NewGameSavePopup.Submitted += HandlePopupSubmitted;
		NewGameSavePopup.Closed += HandlePopupClosed;

		_gameSaveEntityList = _unitOfWork.GameSaveRepository.GetAll().Take(3).ToList();

		Controls = GameSaveBtns.ToList().Concat([ BackBtn ]).ToList();
		
		int len = GameSaveBtns.Length;
		SelectHandlers = new Action[len + 1];
		int i = 0;
		for (i = 0; i < len; i += 1)
		{
			int index = i;
			SelectHandlers[i] = () => HandleGameSaveSelect(index);
		}
		SelectHandlers[i] = HandleBackSelect;
		
		for (int j = 0; j < Controls.Count; j += 1)
		{
			var focusIndex = j; 
			var textBtn = Controls[j];
			textBtn.HandleSelectCallback = SelectHandlers[j];
			textBtn.RequestFocus += () => MoveFocusTarget(focusIndex); 
		}

		RefreshTexts();

		base._Ready();
	}

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
	}
	
	void RefreshTexts()
	{
		Texts = []; 
		for (int h = 0; h < 3; h += 1)
		{
			if (h < _gameSaveEntityList.Count)
			{
				GameSave entity = _gameSaveEntityList[h];
				Texts.Add(entity.Username);
			}
			else
			{
				Texts.Add("ADD a NEW SAVE");
			}
		}
		Texts.Add("BACK");

		for (int j = 0; j < Controls.Count; j += 1)
		{
			var textBtn = Controls[j]; 
			textBtn.ForegroundLabel.Text = string.Format(OPTION_TEMPLATE, MARGIN_INTERVAL * j, Texts[j]);
			textBtn.BackgroundLabel.Text = string.Format(OPTION_TEMPLATE, MARGIN_INTERVAL * j, Texts[j]);
		}
	}

	void HandleGameSaveSelect(int index)
	{
		if (index < _gameSaveEntityList.Count)
		{
			GameSave entity = _gameSaveEntityList[index];
			_crawlStatsService.GameSave = entity;
			_crawlStatsService.EmitRefreshUI();

			// give the recently selected gamer credit for the last crawl
			// var mostRecentCrawlStats = _unitOfWork.CrawlStatsRepository
			// 	.QueryScalar(dbSet => dbSet.OrderByDescending(record => record.Id).FirstOrDefault());
			// if (mostRecentCrawlStats != null)
			// {
			// 	mostRecentCrawlStats.GameSaveId = entity.Id;
			// 	_unitOfWork.CrawlStatsRepository.SaveRepository();
			// }	
		}
		else
		{
			NewGameSavePopup.Open();
			Visible = false;
		}
	}

	void HandleBackSelect()
	{
		var resultPanel = MenuBusiness.PopPanel();
		EmitSignal(SignalName.Open, (int)resultPanel.Id);
	}

	void HandlePopupSubmitted() 
	{
		_gameSaveEntityList = _unitOfWork.GameSaveRepository.GetAll().Take(3).ToList();
		RefreshTexts();
		MoveFocusTarget(_gameSaveEntityList.Count - 1); // Grab focus of new game save
	}
	
	void HandlePopupClosed()
	{
		Visible = true;
	}
}
