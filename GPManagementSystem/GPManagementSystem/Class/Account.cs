using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPManagementSystem.Class
{
    public class Account
    {
        public int AccountId { get; }
        public string AccountName { get; }
        public int AccountType { get; }

        public Account(int accountId, string accountName, int accountType)
        {
            AccountId = accountId;
            AccountName = accountName;
            AccountType = accountType;
        }

        public int GetID()
        {
            return AccountId;
        }

        public override string ToString()
        {
            return $"{AccountName} ({(AccountType == 0 ? "Secretary" : "Doctor")})";
        }
    }

    public class AccountDetails
    {
        public int AccountId { get; }
        public string Username { get; }
        public string AccountName { get; }
        public string DateOfBirth { get; }
        public string PhoneNumber { get; }
        public int AccountType { get; }
        public string Notes { get; }
        public string CreationDate { get; }

        public AccountDetails(int accountId, string username, string accountName, string dateOfBirth, string phoneNumber, int accountType, string notes, string creationDate)
        {
            AccountId = accountId;
            Username = username;
            AccountName = accountName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            AccountType = accountType;
            Notes = notes;
            CreationDate = creationDate;
        }
    }
}