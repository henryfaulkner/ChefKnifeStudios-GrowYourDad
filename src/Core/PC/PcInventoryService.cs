using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IPcInventoryService
{
	void AddToInventory(ItemBase item);
	int CountInventory();
	void Clear();
	
	IEnumerable<ItemWithHealingEffect> GetInvItemsWithHealingEffect();
	IEnumerable<ItemWithBlastingEffect> GetInvItemsWithBlastingEffect();
	IEnumerable<ItemWithPassiveEffect> GetInvItemsWithPassiveEffect();

	int GetPcHpMax();
	int GetPcSpMax();
	int GetPcDamage();
}

public partial class PcInventoryService : GameStateSingletonBase, IPcInventoryService
{
	IPcMeterService _pcMeterService = null!;
	ICrawlStatsService _crawlStatsService = null!;

	List<ItemBase> _itemInventory = new();

	public override void _Ready()
	{
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		base._Ready();
	}

	public void AddToInventory(ItemBase item)
	{
		switch (item)
		{
			case ItemWithHealingEffect itemWithHealingEffect:
				if (_pcMeterService.HpValue + itemWithHealingEffect.OneTimeHpBenefit > _pcMeterService.HpMax)
				{
					_pcMeterService.HpValue = _pcMeterService.HpMax;
				}
				else
				{
					_pcMeterService.HpValue += itemWithHealingEffect.OneTimeHpBenefit;
				}
				break;
			case ItemWithBlastingEffect itemWithBlastingEffect:
				_itemInventory.RemoveAll(x => x is ItemWithBlastingEffect);
				break;
			case ItemWithPassiveEffect itemWithPassiveEffect:
				// when an item is added, which will improve the PC's base health, 
				// their current health should benefit the same amount.
				_pcMeterService.HpValue += itemWithPassiveEffect.BaseHpBenefit;
				break;
			default:
				break;
		}

		_itemInventory.Add(item);
		_crawlStatsService.CrawlStats.ItemsCollected += 1;
	}

	public int CountInventory() => _itemInventory.Count();

	public override void Clear()
	{
		_itemInventory.Clear();
		AddToInventory(new ItemWithBlastingEffect()
		{
			Id = "SINGLE_SHOT_BLASTER",
			Name = "Single Shot Blaster",
			Description = "A blaster that fires one shot at a time.",
			Price = 1,
			DamageBase = 1,
			AmmoConsumed = 1,
			BlasterType = Enumerations.BlasterTypes.SingleShotBlaster,
			RarityTier = "common",
		});
	}

	public IEnumerable<ItemWithHealingEffect> GetInvItemsWithHealingEffect()
	{
		return _itemInventory.OfType<ItemWithHealingEffect>();
	}

	public IEnumerable<ItemWithBlastingEffect> GetInvItemsWithBlastingEffect()
	{
		return _itemInventory.OfType<ItemWithBlastingEffect>();
	}

	public IEnumerable<ItemWithPassiveEffect> GetInvItemsWithPassiveEffect()
	{
		return _itemInventory.OfType<ItemWithPassiveEffect>();
	}

	public int GetPcHpMax()
	{
		int baseHp = 3;
		int additionalHp = GetInvItemsWithPassiveEffect()
			.Sum(x => x.BaseHpBenefit);
		return baseHp + additionalHp;
	}

	public int GetPcSpMax()
	{
		int baseSp = 3;
		int additionalSp = GetInvItemsWithPassiveEffect()
			.Sum(x => x.BaseSpBenefit);
		return baseSp + additionalSp;
	}

	public int GetPcDamage()
	{
		int weaponDamage = GetInvItemsWithBlastingEffect()
			.Sum(x => x.DamageBase);
		int passiveDamage = GetInvItemsWithPassiveEffect()
			.Sum(x => x.DamageBenefit);
		return weaponDamage + passiveDamage;
	}
}
