﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ATMService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ATMService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ATMService.svc or ATMService.svc.cs at the Solution Explorer and start debugging.

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Single)]
    public class ATMService : IATMService
    {
        public List<Account> accounts = new List<Account>();

        public ATMService()
        {
            AddNewAccount(new Account { AccountNumber = 12345678, Balance = 352.00M, Currency = "US", IsProcessing = false });
            AddNewAccount(new Account { AccountNumber = 11223344, Balance = 9957.75M, Currency = "US", IsProcessing = false });
            AddNewAccount(new Account { AccountNumber = 11235813, Balance = 25.00M, Currency = "US", IsProcessing = false });
        }

        public Account AddNewAccount(Account newAccount)
        {
            if (newAccount == null)
            {
                throw new ArgumentNullException("newAccount");
            }

            accounts.Add(newAccount);
            return newAccount;
        }

        public Account GetAccountByAccountNumber(int accountNumber)
        {
            Account resultingAccount = accounts.Find(a => a.AccountNumber == accountNumber);
            return resultingAccount;
        }

        public bool UpdateAccount(Account updatedAccount)
        {
            if (updatedAccount == null)
            {
                throw new ArgumentNullException("updatedAccount");
            }

            int idx = accounts.FindIndex(a => a.AccountNumber == updatedAccount.AccountNumber);
            if (idx == -1)
            {
                return false;
            }

            if (updatedAccount.IsProcessing == false)
            {
                updatedAccount.IsProcessing = true;
            }
            else if (updatedAccount.IsProcessing == true)
            {
                updatedAccount.IsProcessing = false;
            }

            accounts.RemoveAt(idx);
            accounts.Add(updatedAccount);
            return true;
        }

        public RequestResponse Balance(int AccountNumber)
        {
            RequestResponse RequestResult = new RequestResponse();
            Account currentAccount;

            // Get balance of account
            currentAccount = GetAccountByAccountNumber(AccountNumber);

            if (currentAccount != null)
            {
                RequestResult.AccountNumber = currentAccount.AccountNumber;
                RequestResult.Balance = currentAccount.Balance;
                RequestResult.Currency = currentAccount.Currency;
                RequestResult.Successful = true;
                RequestResult.Message = "Current balance for Account Number " + RequestResult.AccountNumber +
                    " is: " + RequestResult.Balance.ToString() + RequestResult.Currency;
            }
            else
            {
                RequestResult.Successful = false;
                RequestResult.Message = "Failed to get balance information for Account Number " + RequestResult.AccountNumber +
                    ". Please confirm the account exists. If you feel you have received this message in error, " +
                    "please contact our technical support team at (877) 000-0000 for assistance.";
            }

            return RequestResult;
        }

        public RequestResponse Deposit(int AccountNumber, decimal Amount, string Currency)
        {
            RequestResponse RequestResult = new RequestResponse();
            Account currentAccount = new Account();
            Account updatedAccount = new Account();

            // Get account information
            currentAccount = GetAccountByAccountNumber(AccountNumber);

            if (currentAccount != null)
            {
                RequestResult.AccountNumber = currentAccount.AccountNumber;
                RequestResult.Balance = currentAccount.Balance + Amount;
                RequestResult.Currency = currentAccount.Currency;

                updatedAccount.AccountNumber = RequestResult.AccountNumber;
                updatedAccount.Balance = RequestResult.Balance;
                updatedAccount.Currency = RequestResult.Currency;
                updatedAccount.IsProcessing = currentAccount.IsProcessing;

                bool processCompletedAndNotDelayed = false;
                int numberOfAttempts = 0;

                while ((processCompletedAndNotDelayed == false) && (numberOfAttempts < 5))
                {
                    if (updatedAccount.IsProcessing == false)
                    {
                        if (UpdateAccount(updatedAccount))
                        {
                            RequestResult.Successful = true;
                            RequestResult.Message = "Current balance for Account Number " + RequestResult.AccountNumber +
                                " is now " + RequestResult.Balance.ToString() + RequestResult.Currency + " after deposit. " +
                                "Please note that funds may not be available for immediate withdrawal.";

                            bool doneDeposit = UpdateAccount(updatedAccount);
                        }
                        else
                        {
                            RequestResult.Successful = false;
                            RequestResult.Message = "Unable to update account balance for Account Number " + RequestResult.AccountNumber +
                            " for deposit transaction. Please contact our technical support team at (877) 000-0000 for assistance.";
                        }

                        processCompletedAndNotDelayed = true;
                    }
                    else
                    {
                        // Another user with access to the account is processing
                        // Wait 2 seconds and try again
                        System.Threading.Thread.Sleep(2000);
                        numberOfAttempts++;
                    }
                }

                if (processCompletedAndNotDelayed == false)
                {
                    RequestResult.Successful = false;
                    RequestResult.Message = "Account Number " + AccountNumber.ToString() +
                        " currently has transactions pending. Deposit is not possible at this time. " +
                        "Please contact our technical support team at (877) 000-0000 for assistance.";
                }
            }
            else
            {
                RequestResult.Successful = false;
                RequestResult.Message = "Failed to get balance information for Account Number " + AccountNumber.ToString() +
                    " to deposit funds. Please confirm the account exists. If you feel you have received this message in error, " +
                    "please contact our technical support team at (877) 000-0000 for assistance.";
            }

            return RequestResult;
        }

        public RequestResponse Withdraw(int AccountNumber, decimal Amount, string Currency)
        {
            RequestResponse RequestResult = new RequestResponse();
            Account currentAccount = new Account();
            Account updatedAccount = new Account();

            // Get account information
            currentAccount = GetAccountByAccountNumber(AccountNumber);

            if (currentAccount != null)
            {
                if (currentAccount.Balance - Amount > 0.00M)
                {
                    RequestResult.AccountNumber = currentAccount.AccountNumber;
                    RequestResult.Balance = currentAccount.Balance - Amount;
                    RequestResult.Currency = currentAccount.Currency;

                    updatedAccount.AccountNumber = RequestResult.AccountNumber;
                    updatedAccount.Balance = RequestResult.Balance;
                    updatedAccount.Currency = RequestResult.Currency;
                    updatedAccount.IsProcessing = currentAccount.IsProcessing;

                    bool processCompletedAndNotDelayed = false;
                    int numberOfAttempts = 0;

                    while ((processCompletedAndNotDelayed == false) && (numberOfAttempts < 5))
                    {
                        if (updatedAccount.IsProcessing == false)
                        {
                            if (UpdateAccount(updatedAccount))
                            {
                                RequestResult.Successful = true;
                                RequestResult.Message = "Current balance for Account Number " + RequestResult.AccountNumber +
                                    " is now " + RequestResult.Balance.ToString() + RequestResult.Currency + " after withdrawal.";

                                bool doneWithdrawal = UpdateAccount(updatedAccount);
                            }
                            else
                            {
                                RequestResult.Successful = false;
                                RequestResult.Message = "Unable to update account balance for Account Number " + RequestResult.AccountNumber +
                                " for withdrawal transaction. Please contact our technical support team at (877) 000-0000 for assistance.";
                            }

                            processCompletedAndNotDelayed = true;
                        }
                        else
                        {
                            // Another user with access to the account is processing
                            // Wait 2 seconds and try again
                            System.Threading.Thread.Sleep(2000);
                            numberOfAttempts++;
                        }
                    }

                    if (processCompletedAndNotDelayed == false)
                    {
                        RequestResult.Successful = false;
                        RequestResult.Message = "Account Number " + AccountNumber.ToString() +
                            " currently has transactions pending. Withdrawal is not possible at this time. " +
                            "Please contact our technical support team at (877) 000-0000 for assistance.";

                        return RequestResult;
                    }                                                           
                }
                else if (currentAccount.Balance - Amount < 0.00M)
                {
                    RequestResult.Successful = false;
                    RequestResult.Message = "Unable to complete withdrawal transaction due to insufficient funds. " +
                        "If you feel you have received this message in error, please contact our technical support team at " +
                        "(877) 000-0000 for assistance.";
                }
            }
            else
            {
                RequestResult.Successful = false;
                RequestResult.Message = "Failed to get balance information for Account Number " + AccountNumber.ToString() +
                    " to withdraw funds. Please confirm the account exists. If you feel you have received this message in error, " +
                    "please contact our technical support team at (877) 000-0000 for assistance.";
            }

            return RequestResult;
        }

        public List<Account> LoadAccounts()
        {
            return accounts;
        }
    }
}
