CREATE TABLE [Position]
(
	[Id] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[RowVersion] ROWVERSION,
	[AccountId] INT NOT NULL,
	[TickerSymbol] NVARCHAR(16) NOT NULL,
	[Quantity] INT NOT NULL,
	[CostBasis] DECIMAL(18,4) NOT NULL,

	CONSTRAINT UK_Position_AccountId_TickerSymbol
	UNIQUE ([AccountId], [TickerSymbol]),

	CONSTRAINT FK_Position_Account
	FOREIGN KEY (AccountId)
	REFERENCES [Account](Id)
);
