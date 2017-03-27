Create Table [dbo].[ListItem]
(
	ListItemId int not null primary key identity(1,1),
	ListId int not null,
	Name nvarchar(255) not null,
	Description nvarchar(max) null,
	Url nvarchar(255) null,
	PictureUrl nvarchar(255) null,
	UserIdPurchased int null ,
	Deleted bit not null,
	UserIdUpdated int not null,
	Updated datetime not null, 
    CONSTRAINT [FK_ListItem_ListId] FOREIGN KEY (ListId) REFERENCES dbo.List([ListId]), 
    CONSTRAINT [FK_ListItem_UserIdPurchased] FOREIGN KEY (UserIdPurchased) REFERENCES [dbo].[User](UserId), 
    CONSTRAINT [FK_ListItem_UserIdUpdated] FOREIGN KEY (UserIdUpdated) REFERENCES [dbo].[User](UserId)
)
