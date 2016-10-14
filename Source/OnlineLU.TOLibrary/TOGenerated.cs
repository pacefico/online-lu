
using System;
using System.Collections.Generic;


namespace OnlineLU.TOLibrary
{
	public class HistoryTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long? TimeCudaMs { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long? TimeDownload { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long? TimeUpload { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime ExecutionDate { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long ID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long? ProjectID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Range { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long? TotalTime { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long? TimeInitialUpalod { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public HistoryDetailTO Detail { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public HardwareTO Hardware { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class GetHistoryParamTO : BaseParamTO
	{

		#region Members


		#endregion Members

		#region Properties

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class GetHistoryRespTO : BaseRespTO
	{

		#region Members

		private IList<HistoryTO> m_Histories = new List<HistoryTO>();

		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public IList<HistoryTO> Histories 
		{
			get{ return this.m_Histories; }
			set
			{
				this.m_Histories = value;
			}
		}

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class HistoryDetailTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public string RateDownload { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string RateUpload { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ByteDownload { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ByteUpload { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string RateInitialUpload { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ByteInitialUpload { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class HardwareTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public long ID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string HardwareKey { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string SystemName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CudaCapable { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int CoreNumber { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ProcessorName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int MemoryAmount { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class ProjectTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public int Range { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ContainerName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ContainerNameResult { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public long ID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string QueueName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string QueueNameResult { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool Status { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public UserTO User { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public HistoryTO History { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class UserTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public long ID { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Password { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class SetHistoryParamTO
	{

		#region Members

		private HistoryTO m_History;

		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public bool IsInitial { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public HistoryTO History 
		{
			get{ return this.m_History; }
			set
			{
				this.m_History = value;
			}
		}

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class SetHistoryRespTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public bool? Success { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class GetExecutionsParamTO : BaseParamTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public DateTime? DateFrom { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime? DateTo { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool? Status { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Range { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class BaseParamTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public int Skip { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Take { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class BaseRespTO
	{

		#region Members


		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public int Total { get; set; }

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

	public class GetExecutionsRespTO : BaseRespTO
	{

		#region Members

		private IList<ProjectTO> m_Projects = new List<ProjectTO>();

		#endregion Members

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public IList<ProjectTO> Projects 
		{
			get{ return this.m_Projects; }
			set
			{
				this.m_Projects = value;
			}
		}

		#endregion Properties

		#region Constructors

		#endregion Constructors

		#region Methods

		#region Implementation

		#endregion Implementation

		#endregion Methods

	}

}


namespace MainModelingProject
{
}
