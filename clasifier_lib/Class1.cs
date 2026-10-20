namespace clasifier_lib;

public enum Category
{
    Food, Transport, Entertainment, Casino, Other
}

public enum CardTier
{
    Standard, Premium
}

public record Merchant
{
    public string Name { get; init; }
    public Category Category { get; set; }
    public string Country { get; init; }

    public Merchant(string name, Category category, string country)
    {
        Name = name;
        Category = category;
        Country = country;
    }
}

public record Account
{
    public string Owner { get; init; }
    public string Country { get; init; }

    public Account(string owner, string country)
    {
        Owner = owner;
        Country = country;
    }
}

public abstract record Transaction
{
    public decimal Amount { get; init; }
    public int Hour { get; init; }
}

public record Payment : Transaction
{
    public Merchant Merchant { get; init; }
    public Payment(decimal amount, int hour, Merchant merchant)
    {
        Amount = amount;
        Hour = hour;
        Merchant = merchant;
    }
}

public record Transfer : Transaction
{
    public Account From { get; init; }
    public Account To { get; init; }

    public Transfer(decimal amount, int hour, Account from, Account to)
    {
        Amount = amount;
        Hour = hour;
        From = from;
        To = to;
    }
    
    public void Deconstruct(out Account? from, out Account? to)
    {
        from = From;
        to = To;
    }
}

public record Deposit : Transaction
{
    public Deposit(decimal amount, int hour)
    {
        Amount = amount;
        Hour = hour;
    }
}

public record Withdrawal : Transaction
{
    public string AtmCountry { get; init; }

    public Withdrawal(decimal amount, int hour, string atmCountry)
    {
        Amount = amount;
        Hour = hour;
        AtmCountry = atmCountry;
    }
}

public static class Classifier
{
    public static bool IsValid(Transaction? t) => t switch
    {
        null => false,
        { Amount: <= 0 } => false,
        { Hour: > 23 } => false,
        { Hour: < 0 } => false,

        Transfer transfer when transfer.From == null => false,
        Transfer transfer when transfer.To == null => false,
        Transfer transfer when transfer.From == transfer.To => false,
        Payment payment when payment.Merchant == null => false,
        _ => true
    };

    public static string Describe(Transaction t) => t switch
    {
        Payment payment => $"Оплата {payment.Amount} в {payment.Merchant.Name}",
        Transfer transfer => transfer switch
        {
            (var from, var to) when from.Owner == to.Owner => $"Перевод себе: {transfer.Amount}",
            _ => $"Перевод {transfer.From.Owner} -> {transfer.To.Owner}: {transfer.Amount}"
        },
        Deposit deposit => $"Пополнение: {deposit.Amount}",
        Withdrawal withdrawal => $"Снятие наличных: {withdrawal.Amount} ({withdrawal.AtmCountry})",
        null => throw new ArgumentNullException("The argument cannot be null"), 
        _ => throw new ArgumentException("An unsuitable type")
    };
}
