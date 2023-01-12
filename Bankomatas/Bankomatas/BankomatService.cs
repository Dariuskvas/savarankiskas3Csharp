//using Bankomatas.Models;

namespace Bankomatas
{
    public class BankomatService
    {
        string returnValue = string.Empty;
        string dbTable = string.Empty;
        string filterCriteria = string.Empty;
        string cardGuidString = string.Empty;
        string creditCardID = string.Empty;


        public void CreateVelcomeMenu()                                                                                                     //PagrintinisMeniu, laukia korteles
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Idekite mokejimo kortele [Prees from 1 to 5 and ENTER]");
                creditCardID = Console.ReadLine();
                if (creditCardID == "1" || creditCardID == "2" || creditCardID == "3" || creditCardID == "4" || creditCardID == "5")
                {
                    Console.WriteLine("Kortele ideta sekmingai, prasome palaukti");
                    Thread.Sleep(2000);
                    Console.Clear();
                    CheckIfCardIsValid(Convert.ToInt32(creditCardID));
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Koretele neideta");
                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(1000);
                    Console.Clear();
                }
            } while (true);
        }


        public void SecondMeniuAfterLogin(int cardID, string cardGUID)                                  //Maniu prisiloginus
        {
            BankService bankService = new BankService();
            Console.Clear();
            Console.WriteLine($"[1] - Saskaitos likutis");
            Console.WriteLine($"[2] - Paskutines 5 tranzakcijos");
            Console.WriteLine($"[3] - Pinigu isemimas");
            Console.WriteLine($"[4] - Iseiti");
            string enterMenuChoise = Console.ReadLine();

            if (enterMenuChoise == "1")                                                     //saskaitos likutis
            {
                Console.Clear();
                Console.WriteLine($"Saskaitos likutis: {bankService.AccountAmount(cardGUID)} EUR");
                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine("Enter kad sugristi");
                Console.ReadLine();
                if (Console.ReadLine != null)
                    SecondMeniuAfterLogin(cardID, cardGUID);
            }


            else if (enterMenuChoise == "2")                                //Paskutines tranzakcijos
            {
                Console.Clear();
                PrintTransaction(creditCardID);
                Console.ReadLine();
                SecondMeniuAfterLogin(cardID, cardGUID);

            }
            else if (enterMenuChoise == "3")                                //Isimti pinigus
            {
                Console.Clear();
                Withdrawmoney(creditCardID, cardGuidString);
                Console.ReadLine();
                SecondMeniuAfterLogin(cardID, cardGUID);
            }
            else if (enterMenuChoise == "4")                                    //Exit to start
            {
                BankomatService bankomatService = new BankomatService();
                bankomatService.CreateVelcomeMenu();
            }
            else
            {
                Console.WriteLine("Blogai pasirinktas meniu, pakartokite pasirinkima");     //Jei klaida kartojam
                Console.WriteLine();
                SecondMeniuAfterLogin(cardID, cardGUID);
            }

        }

        public void CheckIfCardIsValid(int cardID)                              //Tikrinu ar kortele galioja
        {
            dbTable = "BankCards";
            returnValue = "CardValid";
            filterCriteria = "ID";

            if (DBConection.ReadData(DBConection.CreateConnection(), dbTable, filterCriteria, cardID, returnValue) == "1")
                EnterPIN(cardID);
            else
            {
                Console.WriteLine($"Kortele blokuota arba negaliojanti, idekite tinkama kortele");
                Thread.Sleep(2000);
                Console.Clear();
                Program.Main();
            }
        }

        public void EnterPIN(int cardID)                                                                                                      //Tikrinu PIN
        {
            Console.Clear();
            Console.WriteLine("Iveskite PIN koda. [HINT: 1111]");
            int pinTimes = CheckHowManyTimesPinWasEnteredWrong(cardID);
            Console.WriteLine($"PIN kodas ivestas neteisingai {pinTimes} kartus. Liko {3 - pinTimes}");
            string enterPin = Console.ReadLine();
            if (CheckIsItInt(enterPin) == true && CheckPinIsTrue(enterPin) == true)
            {
                DBConection.UpdateData(DBConection.CreateConnection(), "BankAccount", "cardGuid", cardGuidString, "PinEnterIncorect", 0);      //Pakeiciu klaidingai ivestu PIN skaiciu i 0
                SecondMeniuAfterLogin(cardID, cardGuidString);
            }
            else
            {
                DBConection.UpdateData(DBConection.CreateConnection(), "BankAccount", "cardGuid", cardGuidString, "PinEnterIncorect", pinTimes + 1);      //Irasau I DB apie blogai suvesta PIN koda

                if (pinTimes + 1 >= 3)                                                                                                                  // Jei PIN kodas ivestas 3 kartus klaidingai                                                            
                {
                    DBConection.UpdateData(DBConection.CreateConnection(), "BankCards", "cardGuid", cardGuidString, "CardValid", 0);                       //Keiciu statusa i kortele nebegalioja ir grazinu i pradini meniu
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("PIN kodas įvestas neteisingas!");
                    Console.WriteLine("Kortelė blokuota");
                    Thread.Sleep(2000);
                    Console.ForegroundColor = ConsoleColor.White;
                    BankomatService bankomatService = new BankomatService();
                    bankomatService.CreateVelcomeMenu();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("PIN kodas įvestas neteisingas!");
                    Thread.Sleep(2000);
                    Console.ForegroundColor = ConsoleColor.White;
                    EnterPIN(cardID);
                }
            }
        }

        public int CheckHowManyTimesPinWasEnteredWrong(int cardID)                                                                                      //Tikrinu kiek kartu PIN buvo vestas
        {
            cardGuidString = DBConection.ReadData(DBConection.CreateConnection(), "BankCards", "ID", cardID, "CardGuid");
            var wrongPinEnter = DBConection.ReadData(DBConection.CreateConnection(), "BankAccount", "cardGuid", cardGuidString, "PinEnterIncorect");
            return Convert.ToInt32(wrongPinEnter);
        }

        public bool CheckIsItInt(string enteredPIN)                                                                                                     //Pagalbinis metodas
        {
            try
            {
                int enterPinInt = Convert.ToInt32(enteredPIN);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckPinIsTrue(string enterPin)                                                                                                         //Tikrinu ar pin teisingas
        {
            var pinFromDB = DBConection.ReadData(DBConection.CreateConnection(), "BankAccount", "cardGuid", cardGuidString, "cardPIN");
            return enterPin == pinFromDB;

        }

        public void Withdrawmoney(string creditCardID, string cardGuidString)                                                                      //Metodas pinigu isemimui
        {
            BankService bankService = new BankService();
            Console.WriteLine("Kokia suma norite ismti:");
            string amountForTakeOut = Console.ReadLine();
            if (CheckIsItInt(amountForTakeOut) == true && Convert.ToInt32(amountForTakeOut) % 10 == 0)  //Tikrinu ar ivesta suma tinkama
            {
                if (bankService.CheckIfMoneyIsEnofInAccount(Convert.ToInt32(amountForTakeOut), cardGuidString) == true)  //Tikrinu ar pakanka pinigu saskaitoje
                    if (bankService.CheckOperationLimits(cardGuidString, amountForTakeOut) == true)   //Tikrinu ar neisnaudoti operaciju ir pinigu limitai
                    {
                        bankService.MakeRecordAboutTransaction(creditCardID, cardGuidString, Convert.ToInt32(amountForTakeOut));    //Tranzakcijos irasas
                        bankService.WriteOffMoney("BankAccount", "cardGuid", cardGuidString, "amount", Convert.ToInt32(amountForTakeOut));  //Nurasau pinigus is saskaitos
                        bankService.CountOperationLimits(cardGuidString, amountForTakeOut);                                                 //Perskaiciuoju operaciju limitus
                        Console.WriteLine("pinigai isimti sekmingai");
                    }
                    else
                    {
                        Console.WriteLine("Virsiti Limitai");
                    }
                else
                {
                    Console.WriteLine("Neuztenka pinigu");
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Ivesta bloga suma, skaicius turi dalintis is 10");
                Withdrawmoney(creditCardID, cardGuidString);
            }
        }

        public void PrintTransaction(string creditCardID)
        {
            string table = $"ID{creditCardID}Transaction";
            BankService bankService = new BankService();
            bankService.ReadLast(table);

        }

    }
}
