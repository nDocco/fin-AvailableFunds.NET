using System;

namespace FundCalc
{
	/// <summary)>
	/// Base class for Wealth type objects
	/// </summary>
	public abstract class Wealth
	{
		#region Properties
		/// <summary>
		/// An identifier for source of wealth i.e., Bank etc.
		/// </summary>
		public string Entity { get; private set; }
		/// <summary>
		/// A brief description for source of wealth.
		/// </summary>
		public string Reference { get; private set; }
		/// <summary>
		/// The amount of wealth attribute to this instance.
		/// </summary>
		public double Amount { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Provides a default constructor for child classes.
		/// </summary>
		/// <param name="ent"></param>
		/// <param name="rfce"></param>
		/// <param name="amt"></param>
		protected Wealth(string ent, string rfce, double amt)
		{
			Entity = ent;
			Reference = rfce;
			Amount = amt;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Adds given value to amount.
		/// </summary>
		/// <param name="amt"></param>
		public void AddAmount(double amt) { Amount += amt; }
		/// <summary>
		/// Sets the value of amount to 0.0.
		/// </summary>
		public void ClearAmount() { Amount = 0.0; }
		#endregion
	}

	/// <summary>
	/// An object to record savings that contribute to currently available wealth
	/// </summary>
	public class Saving : Wealth
	{
        #region Constructors
        public Saving(string ent, string rfce, double amt) : base(ent, rfce, amt) { }
		#endregion
	}

	/// <summary>
	/// Base class for objects of Wealth Obligation between entities.
	/// </summary>
	public abstract class Obligation : Wealth
	{
        #region Constructors
        protected Obligation(string ent, string rfce, double amt) : base(ent, rfce, amt) { }
        #endregion
    }

    /// <summary>
    /// An object to record Obligations of Wealth by the User to an entity.
    /// </summary>
    public class Debtor : Obligation
	{
        #region Constructors
        public Debtor(string ent, string rfce, double amt) : base(ent, rfce, amt) { }
        #endregion
    }

    /// <summary>
    /// An object to record Obligations of Wealth to the User by an entity.
    /// </summary>
    public class Creditor : Obligation
	{
        #region Constructors
        public Creditor(string ent, string rfce, double amt) : base(ent, rfce, amt) { }
        #endregion
    }

}
