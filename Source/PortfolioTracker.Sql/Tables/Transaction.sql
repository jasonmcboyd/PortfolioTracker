CREATE TABLE [Transaction]
(
	[Id] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[AccountId] INT NOT NULL,
	[TickerSymbol] NVARCHAR(16) NOT NULL,
	[TransactionDate] DATETIME NOT NULL,
	[TransactionType] NVARCHAR(1) NOT NULL,
	[Quantity] INT NOT NULL,
	[Price] DECIMAL(18,4) NOT NULL,

	CONSTRAINT FK_Transaction_Account
	FOREIGN KEY (AccountId)
	REFERENCES [Account](Id)
);
	