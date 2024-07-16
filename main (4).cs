using System;

public class BankAccount
{
    private const decimal MaxWithdrawable = 1000;

    private string _accNo;
    private decimal _balance;

    public BankAccount(decimal initialBalance)
    {
        if (initialBalance < 0)
        {
            Log("Invalid Initial Balance");
        }
        else
        {
            _balance = initialBalance;
        }
    }

    
    public void SetAccNo(string accNo)
    {
        if (string.IsNullOrWhiteSpace(accNo))
        {
            Log("Invalid Account No");
        }
        else
        {
            _accNo = accNo;
        }
    }

    

    public string GetAccNo()
    {
        return _accNo;
    }

    public decimal GetBalance()
    {
        return _balance;
    }

    public decimal Balance
    {
        get { return _balance; }
        set
        {
            if (value < 0)
            {
                Log("Invalid Balance");
            }
            else
            {
                _balance = value;
            }
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Log("Invalid Deposit Amount");
        }
        else
        {
            _balance += amount;
            Log($"Deposited: {amount:C}");
        }
    }

    public event EventHandler OnOverDraft;

    public void Withdraw(decimal amount)
    {
        if (amount > MaxWithdrawable + _balance)
        {
            Log("Rejected: Withdrawal amount exceeds limit");
        }
         else if (_balance < amount)
        {
           OnOverDraft?.Invoke(this, EventArgs.Empty);
            Log("Overdraft Occurred");
        }
        
        else
        {
            _balance -= amount;
            Log($"Withdrew: {amount:C}");
        }
    }

    public static BankAccount operator +(BankAccount a, BankAccount b)
    {
        return new BankAccount(a._balance + b._balance);
    }

    public void Log(string message)
    {
        var time = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
        Console.WriteLine($"[{time}] {message}");
        Console.WriteLine($"[{time}] Balance Info: Account No: '{_accNo}', Balance: '{_balance:C}'");
    }
}

internal class JointAccount
{
    private const int MaxAccountSize = 3;
    private BankAccount[] _accountList = new BankAccount[MaxAccountSize];

    public BankAccount this[int index]
    {
        get
        {
            if (index >= 0 && index < MaxAccountSize)
            {
                return _accountList[index];
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (index >= 0 && index < MaxAccountSize)
            {
                _accountList[index] = value;
            }
        }
    }

    static void Main()
    {
        try
        {
            JointAccount jointAccount = new JointAccount();

            BankAccount acc1 = new BankAccount(100);
            acc1.SetAccNo("123456");
            jointAccount[0] = acc1;

            BankAccount acc2 = new BankAccount(200);
            acc2.SetAccNo("234567");
            jointAccount[1] = acc2;

            BankAccount acc3 = new BankAccount(300);
            acc3.SetAccNo("345678");
            jointAccount[2] = acc3;

            
            jointAccount[0].Deposit(50);
            jointAccount[1].Withdraw(50);
            jointAccount[2].Withdraw(1000); 

            
            for (int i = 0; i < 3; i++)
            {
                if (jointAccount[i] != null)
                {
                    Console.WriteLine($"Account {i + 1} - Account No: {jointAccount[i].GetAccNo()}, Balance: {jointAccount[i].GetBalance():C}");
                }
            }

            
            jointAccount[0].OnOverDraft += (sender, e) => Console.WriteLine($"Overdraft alert for account: {((BankAccount)sender).GetAccNo()}");
            jointAccount[0].Withdraw(200); 

            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
