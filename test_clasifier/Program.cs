using static System.Console;
using clasifier_lib;

class Program
{
    static void Main(string[] args)
    {
        Check("Payment(1500, 14, Merchant(\"Пятёрочка\", Food, \"RU\")", 
            "Оплата 1500 в Пятёрочка", Classifier.Describe(new Payment(1500, 14, new Merchant("Пятёрочка", Category.Food, "RU"))));
        Check("Transfer(5000, 10, Account(\"Анна\",\"RU\"), Account(\"Анна\",\"RU\"))", 
            "Перевод себе: 5000", Classifier.Describe(new Transfer(5000, 10, new Account("Анна","RU"), new Account("Анна","DE"))));
        Check("Transfer(700, 10, Account(\"Борис\",\"RU\"), Account(\"Борис\",\"RU\"))",
            "Перевод Анна -> Борис: 700", Classifier.Describe(new Transfer(700, 10, new Account("Анна","RU"), new Account("Борис","RU"))));
        Check("Deposit(1000, 9)", "Пополнение: 1000" ,Classifier.Describe(new Deposit(1000, 9)));
        Check("Withdrawal(2000, 3, \"TR\")", "Снятие наличных: 2000 (TR)", Classifier.Describe(new Withdrawal(2000, 3, "TR")));
        Throws<ArgumentNullException>("Describe(null) -> ArgumentNullException", () => Classifier.Describe(null));


        Check("IsValid(null) => false", false, Classifier.IsValid(null));
        Check("IsValid(Deposit(1000, 9))", true, Classifier.IsValid(new Deposit(1000, 9)));
        Check("IsValid(Withdrawal(2000, 3, \"TR\"))", true, Classifier.IsValid(new Withdrawal(2000, 3, "TR")));
        Check("Deposit(0, 9)", false, Classifier.IsValid(new Deposit(0, 9)));
        Check("Deposit(-100, 9)", false, Classifier.IsValid(new Deposit(-100, 9)));
        Check("Deposit(0.01m, 9)", true, Classifier.IsValid(new Deposit(0.01m , 9)));
        Check("Deposit(100, -1)", false, Classifier.IsValid(new Deposit(100, -1)));
        Check("Deposit(100, 0)", true, Classifier.IsValid(new Deposit(100, 0)));
        Check("Deposit(100, 23)", true, Classifier.IsValid(new Deposit(100, 23)));
        Check("Deposit(100, 24)", false, Classifier.IsValid(new Deposit(100, 24)));
        Check("Transfer Анна RU => Анна RU", false, 
            Classifier.IsValid(new Transfer(1000, 4, new Account("Анна","RU"), new Account("Анна","RU"))));
        Check("Transfer Анна RU => Анна RU", true, 
            Classifier.IsValid(new Transfer(1000, 4, new Account("Анна","RU"), new Account("Анна","DE"))));
        Check("Transfer Анна RU => Борис RU", true, 
            Classifier.IsValid(new Transfer(1000, 4, new Account("Анна","RU"), new Account("Борис","RU"))));
        Check("Transfer Анна RU => null", false, 
            Classifier.IsValid(new Transfer(1000, 4, new Account("Анна","RU"), null)));
        Check("Payment(.., .., null)", false, 
            Classifier.IsValid(new Payment(5000, 3, null)));
    }   

    static void Throws<TException>(string name, Action action) where TException: Exception
    {
        try
        {
            action();
        }
        catch (ArgumentNullException ex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(name);
            WriteLine($"PASS: {ex.GetType().Name}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(name);
            WriteLine($"FAIL expected: {typeof(TException).Name} got: {ex.GetType().Name}");
            Console.ResetColor();
        }
    }

    static void Check<T>(string name, T expected, T got) where T : IEquatable<T>
    {
        if (expected.Equals(got))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(name);
            WriteLine($"PASS expected: {expected}, got: {got}\n");
            Console.ResetColor();
        }
            
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(name);
            WriteLine($"FAIL expected: {expected}, got: {got}\n");
            Console.ResetColor();
        }
    }
}