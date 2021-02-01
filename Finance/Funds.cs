using System.Collections.Generic;

namespace FundCalc
{
    /// <summary>
    /// Represents all <see cref="Wealth"/> objects contributing to currently available Funds.
    /// </summary>
    public class CurrentFunds
    {
        #region Properties
        /// <summary>
        /// Collection of <see cref="Saving"/> objects that contribute to available wealth.
        /// </summary>
        public List<Saving> Savings { get; private set; }
        /// <summary>
        /// Collection of <see cref="Debtor"/> objects owed by the user.
        /// </summary>
        public List<Debtor> Debtors { get; private set; }
        /// <summary>
        /// Collection of <see cref="Creditor"/> objects owed to the user.
        /// </summary>
        public List<Creditor> Creditors { get; private set; }
        /// <summary>
        /// Represents the total amount of savings across all <see cref="Wealth"/> objects in this instance.
        /// </summary>
        public double Amount { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty instance of CurrentFunds
        /// </summary>
        public CurrentFunds()
        {
            Savings = new List<Saving>();
            Debtors = new List<Debtor>();
            Creditors = new List<Creditor>();
            Amount = 0.0;
        }
        /// <summary>
        /// Loads existing Wealth objects into new instance.
        /// </summary>
        /// <param name="savings"></param>
        /// <param name="debtors"></param>
        /// <param name="creditors"></param>
        public CurrentFunds(List<Saving> savings, List<Debtor> debtors, List<Creditor> creditors)
        {
            Savings = savings;
            Debtors = debtors;
            Creditors = creditors;
            this.CalculateFunds();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sums the savings across all <see cref="Wealth"/> objects and inputs value into Amount for this instance.
        /// </summary>
        public void CalculateFunds()
        {
            double amt = 0.0;
            this.Savings.ForEach(AddToAmt);
            this.Debtors.ForEach(SubFromAmt);
            this.Creditors.ForEach(AddToAmt);

            void AddToAmt(Wealth w) { amt += w.Amount; }
            void SubFromAmt(Wealth w) { amt -= w.Amount; }
            this.Amount = amt;
        }
        // TODO: APPEND METHOD(S)
        // TODO: REMOVE METHOD(S)
        // TODO: TOSTRING METHOD
        #endregion
    }

    /// <summary>
    /// Represents all <see cref="Budget"/> objects expected to require funds allocated.
    /// </summary>
    public class AllocatedFunds
    {
        #region Properties
        /// <summary>
        /// Collection of <see cref="Income"/> objects.
        /// </summary>
        public List<Income> Revenue { get; private set; }
        /// <summary>
        /// Collection of <see cref="Expense"/> objects.
        /// </summary>
        public List<Expense> Expenditure { get; private set; }
        /// <summary>
        /// Represents total amount of savings allocated to paying budgeted items on time.
        /// </summary>
        public double Amount { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an empty instance of AllocatedFunds
        /// </summary>
        public AllocatedFunds()
        {
            Revenue = new List<Income>();
            Expenditure = new List<Expense>();
            Amount = 0.0;
        }
        /// <summary>
        /// Loads existing Budget objects into new instance.
        /// </summary>
        /// <param name="revenues"></param>
        /// <param name="expenses"></param>
        public AllocatedFunds(List<Income> revenues, List<Expense> expenses)
        {
            Revenue = revenues;
            Expenditure = expenses;
            this.CalculateFunds();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculates funds allocated to each <see cref="Budget"/> object for this instance and puts the sum into Amount.
        /// </summary>
        public void CalculateFunds()
        {
            double amt = 0.0;
            this.Revenue.ForEach(SubFromAmt);
            this.Expenditure.ForEach(AddToAmt);

            void AddToAmt(Budget b) { amt += b.Accrued(); }
            void SubFromAmt(Budget b) { amt -= b.Accrued(); }
            this.Amount = amt;
        }
        #endregion
    }

    /// <summary>
    /// Represents available funds from <see cref="CurrentFunds"/> after accounting for <see cref="AllocatedFunds"/>.
    /// </summary>
    public class AvailableFunds
    {
        #region Properties
        public CurrentFunds Current { get; private set; }
        public AllocatedFunds Allocated { get; private set; }
        /// <summary>
        /// Represents available Funds after all commmitments have been deducted from current wealth.
        /// </summary>
        public double Amount { get; private set; }
        #endregion

        #region Constructors
        public AvailableFunds()
        {
            Current = new CurrentFunds();
            Allocated = new AllocatedFunds();
            Amount = 0.0;
        }
        public AvailableFunds(CurrentFunds current, AllocatedFunds allocated)
        {
            Current = current;
            Allocated = allocated;
            this.CalculateFunds();
        }
        #endregion

        #region Methods
        public void CalculateFunds()
        {
            this.Current.CalculateFunds();
            this.Allocated.CalculateFunds();
            this.Amount = this.Current.Amount - this.Allocated.Amount;
        }
        #endregion
    }

}
