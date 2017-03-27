Create Table [dbo].[User]
(
	UserId int not null primary key identity(1,1),
	--UserName nvarchar(255) not null,
	FirstName nvarchar(50) not null,
	LastName nvarchar(50) not null,
	EmailAddress nvarchar(255) null,
	SendEmails bit not null,
	DobDay int not null,
	DobMonth int not null
)
