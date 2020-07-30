CREATE TABLE [Transfer]
(
	[Id] INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[AccountId] INT NOT NULL,
	[Amount] DECIMAL(18,4) NOT NULL,
	[TransferDate] DATETIME NOT NULL,
	[TransferType] NVARCHAR(1) NOT NULL,

	CONSTRAINT FK_Transfer_Account
	FOREIGN KEY (AccountId)
	REFERENCES [Account](Id)
);
