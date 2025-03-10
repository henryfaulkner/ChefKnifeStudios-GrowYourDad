using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IPcInventoryService
{
    List<ItemBase> ItemInventory { get; }
    void AddToInventory(ItemBase item);
    
    IEnumerable<ItemWithHealingEffect> GetInvItemsWithHealingEffect();
    IEnumerable<ItemWithDamagingEffect> GetInvItemsWithDamagingEffect();
    IEnumerable<ItemWithPassiveEffect> GetInvItemsWithPassiveEffect();

    int GetPcHpMax();
    int GetPcSpMax();
    int GetPcDamage();
}

public partial class PcInventoryService : Node, IPcInventoryService
{
    IPcMeterService _pcMeterService;

    public List<ItemBase> ItemInventory { get; private set; } = new();

    public override void _Ready()
    {
        _pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
    }

    public void AddToInventory(ItemBase item)
    {
        if (item is ItemWithHealingEffect itemWithHealingEffect)
        {
            if (_pcMeterService.HpValue + itemWithHealingEffect.OneTimeHpBenefit > _pcMeterService.HpMax)
                _pcMeterService.HpValue = _pcMeterService.HpMax;
            else 
                _pcMeterService.HpValue += itemWithHealingEffect.OneTimeHpBenefit;
            return; 
        }

        ItemInventory.Add(item);
    }

    public IEnumerable<ItemWithHealingEffect> GetInvItemsWithHealingEffect()
    {
        return ItemInventory.OfType<ItemWithHealingEffect>();
    }

    public IEnumerable<ItemWithDamagingEffect> GetInvItemsWithDamagingEffect()
    {
        return ItemInventory.OfType<ItemWithDamagingEffect>();
    }

    public IEnumerable<ItemWithPassiveEffect> GetInvItemsWithPassiveEffect()
    {
        return ItemInventory.OfType<ItemWithPassiveEffect>();
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
        int weaponDamage = GetInvItemsWithDamagingEffect()
            .Sum(x => x.DamageBase);
        int passiveDamage = GetInvItemsWithPassiveEffect()
            .Sum(x => x.DamageBenefit);
        return weaponDamage + passiveDamage;
    }
}
