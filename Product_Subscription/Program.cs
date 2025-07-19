namespace Product_Subscription;

class BaseClass
{
	public Guid Id { get; } = new();
	public DateTime CreatedTimestamp { get; } = DateTime.UtcNow;
}
class Product : BaseClass
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Brand { get; set; }
	public Guid ProductCategoryId { get; set; }
}

class ProductCategory : BaseClass
{
	public string Name { get; set; }
}

class User : BaseClass
{
	public string Name { get; set; }
	public string Email { get; set; }
	public string PhoneNumber { get; set; }
	public bool IsActive { get; set; } = true;
	public bool IsBuyer { get; set; }
	public bool IsSeller { get; set; }
}

class Address : BaseClass
{
	public string Line1 { get; set; }
	public string Line2 { get; set; }
	public string PostalCode { get; set; }
	public string City { get; set; }
	public string State { get; set; }
	public string Country { get; set; }
}

class UserAddress : BaseClass
{
	public Guid UserId { get; set; }
	public Guid AddressId { get; set; }
}

class ProductSeller : BaseClass
{
	public Guid UserId { get; set; }
	public Guid ProductId { get; set; }
	public int Quantity { get; set; }
	public bool IsActive { get; set; } = true;
	public double Price { get; set; }
}

class Subscription : BaseClass
{
	public Guid UserId { get; set; }
	public Guid ProductId { get; set; }
	public Guid SubscriptionTypeId { get; set; }
	public DateTime NextDeliveryDate { get; set; }
	public bool IsPaused { get; set; }
	public bool IsCanceled { get; set; }
	public string? CancelReason { get; set; }
}

class SubscriptionType : BaseClass
{
	public string Name { get; set; }
	public int Days { get; set; }
}