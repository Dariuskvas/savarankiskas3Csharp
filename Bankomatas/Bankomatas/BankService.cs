//using Bankomatas.Models;

namespace Bankomatas
{
    public class BankService
    {
        public int AccountAmount(string guid)                                                                                               //skirta pasitikrinti kiek pinigu saskaitoje
        {
            var amountFromDB = DBConection.ReadData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "amount");
            return Convert.ToInt32(amountFromDB);
        }

        public bool CheckIfMoneyIsEnofInAccount(int enterAmount, string guid)
        {
            int accountAmount = AccountAmount(guid);
            return accountAmount > enterAmount;

        }

        public bool CheckOperationLimits(string guid, string amountForTakeOut)                                                  
        {
            string cardID = DBConection.ReadData(DBConection.CreateConnection(), "BankCards", "cardGuid", guid, "ID");
            string tableName = $"ID{cardID}Transaction";
            var lastOperationDate = DateTime.Parse(DBConection.ReadData(DBConection.CreateConnection(), tableName, guid)).Date;

            if (lastOperationDate < DateTime.Now.Date)                                                                                  //Jei poaskutines operacijos data yra senesne nei dabar, atnaujinu operaciju limitus
            {
                DBConection.UpdateData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayAmountLimits", 1000);
                DBConection.UpdateData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayOperationLimits", 10);
            }

            var dayAmountLimits = DBConection.ReadData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayAmountLimits");                     //Tikrinu limitus
            var dayOperationLimits = DBConection.ReadData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayOperationLimits");
            if (Convert.ToInt32(dayAmountLimits) >= Convert.ToInt32(amountForTakeOut) && Convert.ToInt32(dayOperationLimits) >= 1)
                return true;
            else
                return false;
        }

        public void CountOperationLimits(string guid, string moneyTakeOut)
        {
            var dayAmountLimits = DBConection.ReadData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayAmountLimits");
            var dayOperationLimits = DBConection.ReadData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayOperationLimits");
            DBConection.UpdateData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayAmountLimits", Convert.ToInt32(dayAmountLimits) - Convert.ToInt32(moneyTakeOut));
            DBConection.UpdateData(DBConection.CreateConnection(), "BankAccount", "cardGuid", guid, "dayOperationLimits", Convert.ToInt32(dayOperationLimits) - 1);
        }

        public void MakeRecordAboutTransaction(string tableName, string cardGuidString, int amountForTakeOut)
        {
            string newTableName = $"ID{tableName}Transaction";
            string operationDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                DBConection.CreateTable(DBConection.CreateConnection(), newTableName, "CardGuid", "operationDate", "operationAmount");
            }
            catch { }
            finally
            {

                DBConection.InsertData(DBConection.CreateConnection(), newTableName, cardGuidString, operationDate, amountForTakeOut);

            }
        }

        public void WriteOffMoney(string dbTable, string filterCriteria, string filterValue, string newCriteria, int newValue)
        {
            int accountAmmount = AccountAmount(filterValue);
            int moneyLeftInAccount = accountAmmount - newValue;
            DBConection.UpdateData(DBConection.CreateConnection(), dbTable, filterCriteria, filterValue, newCriteria, moneyLeftInAccount);
        }

        public void ReadLast(string table)
        {
            DBConection.ReadData(DBConection.CreateConnection(), table);
        }
    }
}
