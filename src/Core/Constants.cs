public partial class Constants
{
	public partial class Config
	{
		public const string ConnectionString = "Data Source=app_database.db;";
	}

	public partial class SingletonNodes
	{
		public const string LoggerService = "/root/LoggerService";
		public const string PcMeterService = "/root/PcMeterService";
		public const string PcWalletService = "/root/PcWalletService";
		public const string PcInventoryService = "/root/PcInventoryService";
		public const string PcPositionService = "/root/PcPositionService";
		public const string Observables = "/root/Observables";
		public const string BlasterFactory = "/root/BlasterFactory";
		public const string BlastFactory = "/root/BlastFactory";
		public const string ProteinFactory = "/root/ProteinFactory";
		public const string EnemyFactory = "/root/EnemyFactory";
		public const string ShopKeeperService = "/root/ShopKeeperService";
		public const string CrawlStatsService = "/root/CrawlStatsService";
		public const string UnitOfWork = "/root/UnitOfWork";
		public const string NavigationAuthority = "/root/NavigationAuthority";
	}

	public partial class Invulnerability
	{
		public const float Duration = 1.0f;
		public const float Interval = 0.2f;
	}

	public partial class EnemyControllerNodes
	{
		public const string CircleFish = "CircleFish";
		public const string LineFish = "LineFish";
		public const string PathFindingFish = "PathFindingFish";
	}
}
