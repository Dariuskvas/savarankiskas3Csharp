// See https://aka.ms/new-console-template for more information
using Bankomatas;

public class Program
{
    public static void Main()                  //Pradinis MENIU ideti kortele
    {
        BankomatService bankomatService = new BankomatService();
        bankomatService.CreateVelcomeMenu();
    }
}