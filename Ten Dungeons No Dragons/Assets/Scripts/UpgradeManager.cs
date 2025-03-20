using System.Collections.Generic;
using UnityEngine;
using Singleton;

public enum UpgradeType
{
    Rage,
    ExtraAttack,
    SecondWind,
    HuntersMark,
    NaturesVeil,
    Dash,
    Evasion
}

public class UpgradeManager : Singleton<UpgradeManager>
{
    private List<UpgradeType> ownedUpgrades = new List<UpgradeType>(0);

    private void Awake()
    {
        SetInstance(this);
        DontDestroyOnLoad(this);
    }

    public void AddUpgradeItem(UpgradeType upgrade)
    {
        ownedUpgrades.Add(upgrade);
    }

    public List<UpgradeType> GetOwnedUpgrades()
    {
        return ownedUpgrades;
    }

    public void ResetUpgrades()
    {
        ownedUpgrades.Clear();
    }
}