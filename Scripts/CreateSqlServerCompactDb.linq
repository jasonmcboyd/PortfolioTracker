<Query Kind="Statements">
  <Connection>
    <ID>ca7bb464-e599-46f2-a208-a0a20c420096</ID>
    <Persist>true</Persist>
    <Provider>System.Data.SqlServerCe.4.0</Provider>
    <AttachFileName>C:\wrk\temp\PortfolioTracker.sdf</AttachFileName>
  </Connection>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationCore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationFramework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.IO.Compression.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.IO.Compression.FileSystem.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\WindowsBase.dll</Reference>
  <NuGetReference>FSharp.Charting</NuGetReference>
  <NuGetReference>FSharp.Data</NuGetReference>
  <NuGetReference>Microsoft.SqlServer.Compact</NuGetReference>
  <NuGetReference>Mono.Cecil</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>System.Interactive</NuGetReference>
  <NuGetReference>System.Memory</NuGetReference>
  <NuGetReference>System.Reactive</NuGetReference>
  <Namespace>FSharp.Charting</Namespace>
  <Namespace>FSharp.Data</Namespace>
  <Namespace>Mono.Cecil</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Bson</Namespace>
  <Namespace>Newtonsoft.Json.Converters</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Schema</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Data.SqlServerCe</Namespace>
  <Namespace>System.IO.Compression</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows</Namespace>
  <Namespace>System.Windows.Controls</Namespace>
</Query>

string databasePath = @"c:\wrk\temp\PortfolioTracker.sdf";
string connectionString = $"Data Source={databasePath}";
string schemaPath = @"C:\wrk\dev\personal\PortfolioTracker\Source\PortfolioTracker.Sql";
string dataPath = @"C:\wrk\dev\personal\PortfolioTracker\Data";
string accountDataPath = $@"{dataPath}\Account.csv";
string transactionDataPath = $@"{dataPath}\Transaction.csv";
string transferDataPath = $@"{dataPath}\Transfer.csv";


var indexes = Directory.GetFiles($@"{schemaPath}\Tables", "IX_*");
var tables = Directory.GetFiles($@"{schemaPath}\Tables").Except(indexes);

File.Delete(databasePath);

using (SqlCeEngine engine = new SqlCeEngine(connectionString))
{
	engine.CreateDatabase();
}

using (SqlCeConnection connection = new SqlCeConnection(connectionString))
using (SqlCeCommand command = connection.CreateCommand())
{
	connection.Open();
	foreach (var table in tables)
	{
		command.CommandText = File.ReadAllText(table);
		command.ExecuteNonQuery();
	}
	foreach (var index in indexes)
	{
		command.CommandText = File.ReadAllText(index);
		command.ExecuteNonQuery();
	}
	foreach (var account in File.ReadLines(accountDataPath).Skip(1))
	{
		var values = account.Split(new char[] { ',' });
		
		command.CommandText = $@"
			INSERT INTO [Account]([FirstName],[LastName])
			VALUES('{values[0]}', '{values[1]}')";
		command.ExecuteNonQuery();
	}
	foreach (var transaction in File.ReadLines(transactionDataPath).Skip(1))
	{
		var values = transaction.Split(new char[] { ',' });
		command.CommandText = $@"
			INSERT INTO [Transaction]([AccountId],[TickerSymbol],[TransactionDate],[TransactionType],[Quantity],[Price])
			VALUES({int.Parse(values[0])}, '{values[1]}', '{values[2]}', '{values[3]}', {int.Parse(values[4])}, {decimal.Parse(values[5])})";
		command.ExecuteNonQuery();
	}
	foreach (var transfer in File.ReadLines(transferDataPath).Skip(1))
	{
		var values = transfer.Split(new char[] { ',' });
		command.CommandText = $@"
			INSERT INTO [Transfer]([AccountId],[TransferDate],[TransferType],[Amount])
			VALUES({int.Parse(values[0])}, '{values[1]}', '{values[2]}', {decimal.Parse(values[3])})";
		command.ExecuteNonQuery();
	}
}

Transactions
.OrderBy(x => x.TransactionDate)
.ForEach(x => 
{
	var position = Positions.FirstOrDefault(y => x.TickerSymbol == y.TickerSymbol);
	if (position == null)
	{
		Positions.InsertOnSubmit(new Position
		{
			AccountId = 1,
			Quantity = x.Quantity,
			TickerSymbol = x.TickerSymbol,
			CostBasis = x.Quantity * x.Price
		});
	}
	else
	{
		if (x.TransactionType == "B")
		{
			position.Quantity += x.Quantity;
			position.CostBasis += x.Quantity * x.Price;
		}
		else
		{
			position.Quantity -= x.Quantity;
			if (position.Quantity == 0)
			{
				position.CostBasis = 0;
			}
			else
			{
				position.CostBasis -= x.Quantity * x.Price;	
			}
		}
	}
	SubmitChanges();
});

Transfers
.Select(x => new
{
	Date = x.TransferDate,
	x.Amount,
	Multiplier = 1,
	Source = "Transfer"
})
.Concat(
	Transactions
	.Select(x => new
	{
		Date = x.TransactionDate,
		Amount = x.Quantity * x.Price,
		Multiplier = x.TransactionType == "B" ? -1 : 1,
		Source = "Transaction"
	}))
.OrderBy(x => x.Date)
.ForEach(x => 
{
	var account = Accounts.First();
	account.Balance += x.Amount * x.Multiplier;
	SubmitChanges();
});