Create Table [dbo].[ListType]
(
	ListTypeId int not null primary key identity(1,1),
	Code nvarchar(50) not null,
	[Name] nvarchar(255) not null
)
