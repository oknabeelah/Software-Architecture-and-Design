using System;
using System.Collections.Generic;
	namespace SAaD; 
		class Program 
		{ 
			static void Main() 
			{
                // new checkout serfvice
                CheckoutService checkout = new CheckoutService();

                IPaymentMethod card = new CreditCardPayment("1234-5678-9012-3456", "John Doe");
                IPaymentMethod paypal = new PayPalPayment("john.doe@example.com");
                IPaymentMethod bank = new BankTransferPayment("20-00-00", "12345678");

                Console.WriteLine("Invocations via checkout service:");
                checkout.ProcessPayment(card, 100.0);
                checkout.ProcessPayment(paypal, 200.0);
                checkout.ProcessPayment(bank, 300.0);

                Console.WriteLine("Batch processing of payments:");
                List<(IPaymentMethod Method, double Amount)> pendingTransactions = new List<(IPaymentMethod Method, double Amount)>
                {
                    (card, 100.0),
                    (paypal, 200.0),
                    (bank, 300.0)
                };

                double totalProcessed = 0;
                foreach (var transaction in pendingTransactions)
                {
                    transaction.Method.Pay(transaction.Amount);
                    totalProcessed += transaction.Amount;
                }
                Console.WriteLine(new string('-', 58));
                Console.WriteLine($"{"Total Processed:",-44} £{totalProcessed:N2}");
                Console.WriteLine("==========================================================");
            }
        }

        public class CheckoutService()
        {
            public void ProcessPayment(IPaymentMethod method, double amount)
            {
                if (method == null)
                {
                    throw new ArgumentNullException(nameof(method), "Payment method cannot be null.");
                }
                if (amount <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
                }
                Console.WriteLine($"Processing payment of {amount} using {method.GetType().Name}.");
                method.Pay(amount);
            }
        }

        public interface IPaymentMethod
        {
            void Pay(double amount);
            //pay method should print the payment method and the amount paid
        }

        public class CreditCardPayment : IPaymentMethod
        {
            private string cardNumber;
            private string cardHolderName;
            public CreditCardPayment(string cardNumber, string cardHolderName)
            {
                this.cardNumber = cardNumber;
                this.cardHolderName = cardHolderName;
            }
            public void Pay(double amount)
            {
                string maskedCardNumber = "****-****-****-" + cardNumber.Substring(cardNumber.Length - 4);
                Console.WriteLine($" {cardHolderName} Paid {amount} using Credit Card ending in {maskedCardNumber}.");
            }
        }

        public class PayPalPayment : IPaymentMethod
        {
            private string email;
            public PayPalPayment(string email)
            {
                this.email = email;
            }   
            public void Pay(double amount)
            {
                Console.WriteLine($"Paid {amount} using PayPal account {email}.");
            }
        }

        public class BankTransferPayment : IPaymentMethod
        {
            private string sortCode;
            private string accountNumber;
            public BankTransferPayment(string sortCode, string accountNumber)
            {
                this.sortCode = sortCode;
                this.accountNumber = accountNumber;
            }
            public void Pay(double amount)
            {
                Console.WriteLine($"Paid {amount} using Bank Transfer (Sort Code: {sortCode}, Account Number: {accountNumber}).");
            }
        }
internal class List<T1, T2, T3, T4>
{
}