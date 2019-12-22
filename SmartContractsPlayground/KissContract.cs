using System;
using Stratis.SmartContracts;

[Deploy]
public class KissContract : SmartContract
{
    private const string PurchasedItemsCountKey = "purchased items count";
    private const int MaxDiscountPercent = 5;
    private const int StartPrice = 15000;

    public KissContract(ISmartContractState state) : base(state)
    {
        PurchasedItemsCount = 0;
    }

    private uint PurchasedItemsCount
    {
        get
        {
            return this.PersistentState.GetUInt32(PurchasedItemsCountKey);
        }

        set
        {
            this.PersistentState.SetUInt32(PurchasedItemsCountKey, value);
        }
    }

    public void IncrementCount(uint val)
    {
        var newCount = PurchasedItemsCount + val;
        newCount = newCount >= 0 ? newCount : 0;
        PurchasedItemsCount = newCount;
    }

    public uint EvaluatePrice()
    {
        var itemDiscount = PurchasedItemsCount / 100;
        if (itemDiscount > MaxDiscountPercent)
        {
            itemDiscount = MaxDiscountPercent;
        }

        return (StartPrice / 100) * (100 - itemDiscount);
    }

    public uint GetItemCount() => PurchasedItemsCount;
}
