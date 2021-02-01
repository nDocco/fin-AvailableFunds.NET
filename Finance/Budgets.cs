using System;

namespace Budgets
{
    #region PayPeriod
    /// <summary>
    /// Defines the type of payment period
    /// </summary>
    public enum PayPeriod : byte
    {
        Regular,
        Weekly,
        Fortnightly,
        Monthly,
        Annually,
    }
    /// <summary>
    /// Extension of PayPeriod with methods to support operations on DateTime types.
    /// </summary>
    public static class PayPeriodExtension
    {
        /// <summary>
        /// Adjusts the date by a given payment period.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="frequency"></param>
        /// <param name="period"></param>
        /// <returns>A DateTime with value of this instance adjusted by the frequency of period.</returns>
        public static DateTime AddPeriod(this DateTime date, int frequency, PayPeriod period)
        {
            switch (period)
            {
                case PayPeriod.Regular:
                    date = date.AddDays(frequency);
                    return date;
                case PayPeriod.Weekly:
                    date = date.AddDays(frequency * 7);
                    return date;
                case PayPeriod.Fortnightly:
                    date = date.AddDays(frequency * 14);
                    return date;
                case PayPeriod.Monthly:
                    date = date.AddMonths(frequency);
                    return date;
                case PayPeriod.Annually:
                    date = date.AddYears(frequency);
                    return date;
                default:
                    return date;
            }
        }
    }
    #endregion

    /// <summary>
    /// Base class for budget type objects used to predict or determine finances.
    /// </summary>
    public abstract class Budget
    {
        #region Properties
        /// <summary>
        /// A brief description of source of income or commitment.
        /// </summary>
        public string Reference { get; private set; }
        /// <summary>
        /// The amount due on each payment period.
        /// </summary>
        public double Amount { get; private set; }
        /// <summary>
        /// The number of periods between payments.
        /// </summary>
        public int Frequency { get; private set; }
        /// <summary>
        /// The type of period represented by <see cref="Frequency"/>.
        /// <para>Available types are defined by <see cref="PayPeriod"/>.</para>
        /// </summary>
        public PayPeriod Period { get; private set; }
        /// <summary>
        /// The date of the first payment.
        /// </summary>
        public DateTime StartDate { get; private set; }
        /// <summary>
        /// The date when payments will cease.
        /// </summary>
        public DateTime EndDate { get; private set; }
        /// <summary>
        /// When enabled payments will only be made on next available weekday.
        /// <para>False by default.</para>
        /// </summary>
        public bool Weekdays { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Provides a default constructor for child classes
        /// </summary>
        /// <param name="src"></param>
        /// <param name="amt"></param>
        /// <param name="frq"></param>
        /// <param name="per"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="wd"></param>
        protected Budget(string src, double amt, int frq, PayPeriod per, DateTime? start = null, DateTime? end = null, bool wd = false)
        {
            Reference = src;
            Amount = amt;
            Frequency = frq;
            Period = per;
            StartDate = start is null ? DateTime.MinValue : (DateTime)start;
            EndDate = end is null ? DateTime.MaxValue : (DateTime)end;
            Weekdays = wd;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculates the amount accrued to date if commitment is to be paid on time.
        /// <para>This is calculated from last payment date or start date whichever is later.</para>
        /// </summary>
        /// <param name="toDate"></param>
        /// <returns>A value of the amount accrued up to and including toDate</returns>
        public double Accrued(DateTime toDate)
        {
            DateTime? start = this.LastDue(toDate);
            // Catch payments that haven't started.
            if (start == null)
            {
                // Set saving period to one payment period prior to start
                start = this.StartDate.AddPeriod(-1 * this.Frequency, this.Period);
                // Check start date is not in the future
                if (start > toDate) { return 0.0; }
            }
            DateTime? due = this.NextDue(toDate);
            // Catch payments that have finished.
            if (due == null) { return 0.0; }

            TimeSpan period = (DateTime)due - (DateTime)start;
            TimeSpan elapsed = toDate - (DateTime)start;
            double accrued = this.Amount * ((double)elapsed.Days / (double)period.Days);
            return accrued;
        }
        public double Accrued()
        {
            DateTime toDate = DateTime.Now;
            return this.Accrued(toDate);
        }

        /// <summary>
        /// Calculates the last payment date for this instance.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns>The last date for payment or null if payments have not begun</returns>
        public DateTime? LastDue(DateTime fromDate)
        {
            // Have payments started?
            if (fromDate < this.StartDate) { return null; }
            // Have payments finished?
            if (fromDate > this.EndDate) { return this.EndDate; }

            // Initialise to first payment and no payments
            DateTime payDate = this.StartDate;
            int payments = 0;
            // Calculate last payment date before fromDate
            switch (this.Period)
            {
                case PayPeriod.Regular:
                    payments = fromDate.Subtract(payDate).Days / this.Frequency;

                    payDate = payDate.AddDays(this.Frequency * payments);
                    return payDate;
                case PayPeriod.Weekly:
                    // Convert Weekly into days.
                    int weekdays = this.Frequency * 7;

                    int days = fromDate.Subtract(payDate).Days;
                    payments = fromDate.Subtract(payDate).Days / weekdays;

                    payDate = payDate.AddDays(weekdays * payments);
                    return payDate;
                case PayPeriod.Fortnightly:
                    // Convert Fortnightly into days.
                    int fortdays = this.Frequency * 14;

                    payments = fromDate.Subtract(payDate).Days / fortdays;

                    payDate = payDate.AddDays(fortdays * payments);
                    return payDate;
                case PayPeriod.Monthly:
                    payments = (fromDate.Month - payDate.Month) + ((fromDate.Year - payDate.Year) * 12);
                    payments = payments / this.Frequency;
                    //Is there still a payment to make this Month?
                    if (fromDate.Day < payDate.Day) { payments--; }

                    payDate = payDate.AddMonths(this.Frequency * payments);
                    return payDate;
                case PayPeriod.Annually:
                    payments = (fromDate.Year - payDate.Year) / this.Frequency;
                    //Is there still a payment to make this Year?
                    if (fromDate.DayOfYear < payDate.DayOfYear) { payments--; }

                    payDate = payDate.AddYears(this.Frequency * payments);
                    return payDate;
                default:
                    return payDate;
            }
        }
        public DateTime? LastDue()
        {
            DateTime fromDate = DateTime.Now;
            return this.LastDue(fromDate);
        }

        /// <summary>
        /// Calculates the next payment date for this instance.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns>The next date for payment or null if there are no more payments</returns>
        public DateTime? NextDue(DateTime fromDate)
        {
            // Have payments started?
            if (fromDate < this.StartDate) { return this.StartDate; }
            // Have payments finished?
            if (fromDate > this.EndDate) { return null; }

            // Initialise to last payment
            DateTime payDate = (DateTime)this.LastDue(fromDate);
            // Calculate next date after fromDate
            payDate = payDate.AddPeriod(this.Frequency, this.Period);
            if (payDate > this.EndDate)
            { return this.EndDate; }
            else
            { return payDate; }
        }
        public DateTime? NextDue()
        {
            DateTime fromDate = DateTime.Now;
            return this.NextDue(fromDate);
        }
        #endregion
    }

    /// <summary>
    /// A Budget Income object used to predict or determine sources of income.
    /// </summary>
    public class Income : Budget
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a budget income object.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="amt"></param>
        /// <param name="frq"></param>
        /// <param name="per"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="wd"></param>
        public Income(string src, double amt, int frq, PayPeriod per, DateTime? start = null, DateTime? end = null, bool wd = false)
            : base(src, amt, frq, per, start, end, wd) { }
        #endregion
    }

    /// <summary>
    /// A Budget Expense object used to predict or determine sources of expense.
    /// </summary>
    public class Expense : Budget
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a budget expense object.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="amt"></param>
        /// <param name="frq"></param>
        /// <param name="per"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="wd"></param>
        public Expense(string src, double amt, int frq, PayPeriod per, DateTime? start = null, DateTime? end = null, bool wd = false)
            : base(src, amt, frq, per, start, end, wd) { }
        #endregion
    }
}