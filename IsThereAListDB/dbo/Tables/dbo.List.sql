Create Table dbo.List
(
	ListId int not null primary key identity(1,1),
	ListTypeId int not null,
	[Name] nvarchar(255) not null,
	UserIdOwner int not null,
	EffectiveDate datetime not null,
	Updated datetime not null, 
    CONSTRAINT [FK_List_ListTypeId] FOREIGN KEY (ListTypeId) REFERENCES dbo.ListType(ListTypeId)
)
