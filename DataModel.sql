create table [Image]
(
	ContentId varchar(30) not null,
	StorageLocation varchar(max) not null,
	[Description] nvarchar(max) not null,
	AltText nvarchar(max) not null,
	Localization varchar(6) not null,
	Width int not null,
	Height int not null,
	[Format] varchar(max) not null,
	ImageAnalysis nvarchar(max),
	Xmp xml,
	CreatedDate datetime2(2) not null constraint DF_Image_CreatedDate default getutcdate(),
	CreatedBy varchar(100) not null constraint DF_Image_CreatedBy default CURRENT_USER,
	constraint PK_Image primary key clustered(ContentId)
)
go