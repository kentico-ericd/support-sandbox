namespace DancingGoat.Commerce;

/// <summary>
/// Represents the data model for the shopping cart API.
/// </summary>
public sealed class ShoppingCartDataModel
{
    /// <summary>
    /// Items inside the shopping cart.
    /// </summary>
    public ICollection<ShoppingCartDataItem> Items { get; init; } = [];


    /// <summary>
    /// Entered coupon codes.
    /// </summary>
    public ICollection<string> CouponCodes { get; init; } = [];
}
