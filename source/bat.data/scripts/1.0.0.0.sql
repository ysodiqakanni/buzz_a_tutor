GO
CREATE TABLE [dbo].[Account](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](300) NOT NULL,
	[Password] [varchar](128) NOT NULL,
	[AccountType_ID] [int] NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

go
insert into account
select 'steve', 'steve', 1

go
insert into account
select 'alex', 'alex', 2

GO
ALTER TABLE dbo.Account ADD
	Fname varchar(500) NULL,
	Lname varchar(500) NULL

go
update account set fname = 'Steve', lname = 'Shearn' where email = 'steve'

go
update account set fname = 'Alex', lname = 'Ho-Terry' where email = 'alex'


go
insert into account
select 'sam', 'sam', 1, 'Sam', 'Eames'

GO
CREATE TABLE [dbo].[Lesson](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Account_ID] [int] NOT NULL,
	[BookingDate] [datetime] NOT NULL,
	[DurationMins] [int] NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[ClassSize] [int] NOT NULL,
 CONSTRAINT [PK_Lesson] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

GO
ALTER TABLE dbo.Lesson ADD CONSTRAINT
	FK_Lesson_Account FOREIGN KEY
	(
	Account_ID
	) REFERENCES dbo.Account
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 


GO

CREATE TABLE [dbo].[LessonParticipant](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Lesson_ID] [int] NOT NULL,
	[Account_ID] [int] NOT NULL,
 CONSTRAINT [PK_LessonParticipant] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
) 

GO

ALTER TABLE [dbo].[LessonParticipant]  WITH CHECK ADD  CONSTRAINT [FK_LessonParticipant_Lesson] FOREIGN KEY([Lesson_ID])
REFERENCES [dbo].[Lesson] ([ID])
GO

ALTER TABLE [dbo].[LessonParticipant] CHECK CONSTRAINT [FK_LessonParticipant_Lesson]





GO
CREATE TABLE [dbo].[AccountAttachment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Account_ID] [int] NOT NULL,
	[Title] [varchar](500) NOT NULL,
	[Data] [varchar](max) NOT NULL,
 CONSTRAINT [PK_LessonAttachment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


GO
ALTER TABLE [dbo].[AccountAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AccountAttachment_Account] FOREIGN KEY([Account_ID])
REFERENCES [dbo].[Account] ([ID])



go
drop table [AccountAttachment]

GO
CREATE TABLE [dbo].[LessonAttachment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Lesson_ID] [int] NOT NULL,
	[Account_ID] [int] NOT NULL,
	[Title] [varchar](500) NOT NULL,
	[Data] [varchar](max) NOT NULL,
 CONSTRAINT [PK_LessonAttachment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE dbo.LessonAttachment ADD CONSTRAINT
	FK_LessonAttachment_Lesson FOREIGN KEY
	(
	Lesson_ID
	) REFERENCES dbo.Lesson
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	

GO
ALTER TABLE dbo.Lesson ADD
	TokBoxSessionId varchar(128) NULL

go
update lesson set tokboxsessionid = '2_MX40NTQ5NjY1Mn5-MTQ1ODI1NjE1Nzk0OH5OTnRSTUR5c0FZMnpSYkFob1doR2xNT3h-UH4'






GO
CREATE TABLE [dbo].[FamilyMember](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Account_ID] [int] NOT NULL,
	[Parent_ID] [int] NOT NULL,
 CONSTRAINT [PK_FamilyMember] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


GO
ALTER TABLE [dbo].[FamilyMember]  WITH CHECK ADD  CONSTRAINT [FK_FamilyMember_Account] FOREIGN KEY([Account_ID])
REFERENCES [dbo].[Account] ([ID])

GO
ALTER TABLE [dbo].[FamilyMember]  WITH CHECK ADD  CONSTRAINT [FK_FamilyMember_Parent] FOREIGN KEY([Parent_ID])
REFERENCES [dbo].[Account] ([ID])




GO
ALTER TABLE dbo.Lesson ADD
	ZoomStartUrl varchar(max) NULL

GO
ALTER TABLE dbo.Lesson ADD
	ZoomJoinUrl varchar(max) NULL

GO
CREATE TABLE [dbo].[ChatRecord](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Lesson_ID] [int] NOT NULL,
	[Chat_User] [varchar](500) NOT NULL,
	[Char_Message] [varchar](max) NOT NULL,
	[DateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ChatRecord]  WITH CHECK ADD  CONSTRAINT [FK_ChatRecord_Lesson] FOREIGN KEY([Lesson_ID])
REFERENCES [dbo].[Lesson] ([ID])
GO

ALTER TABLE [dbo].[ChatRecord] CHECK CONSTRAINT [FK_ChatRecord_Lesson]
GO


GO
ALTER TABLE dbo.Lesson ADD
	Subject varchar(500) NULL

GO
ALTER TABLE dbo.Account ADD
	ZoomUserId varchar(50) NULL

	
GO
ALTER TABLE dbo.Lesson ADD
	DetailedDescription varchar(max) NULL


GO
CREATE TABLE [dbo].[LessonResource](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Lession_ID] [int] NOT NULL,
	[Original_Name] [varchar](max) NOT NULL,
	[Item_Storage_Name] [varchar](500) NOT NULL,
 CONSTRAINT [PK_LessonResources] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[LessonResource]  WITH CHECK ADD  CONSTRAINT [FK_LessonResource_Lesson] FOREIGN KEY([Lession_ID])
REFERENCES [dbo].[Lesson] ([ID])

GO
ALTER TABLE [dbo].[LessonResource] CHECK CONSTRAINT [FK_LessonResource_Lesson]
GO

GO
ALTER TABLE dbo.Account ADD
	Description varchar(500) NULL,
	Qualifications varchar(500) NULL,
	Rate int NULL,
	Picture varchar(MAX) NULL
GO

CREATE TABLE [dbo].[SubjectDescription](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [varchar](500) NOT NULL,
	[Description] [varchar](max) NOT NULL,
 CONSTRAINT [PK_SubjectDescription] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


CREATE TABLE [dbo].[SubjectExamPaper](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectDescription_ID] [int] NOT NULL,
	[StorageName] [varchar](max) NOT NULL,
	[Original_Name] [varchar](max) NOT NULL,
 CONSTRAINT [PK_SubjectExamPaper] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[SubjectExamPaper]  WITH CHECK ADD  CONSTRAINT [FK_SubjectExamPaper_SubjectDescription] FOREIGN KEY([SubjectDescription_ID])
REFERENCES [dbo].[SubjectDescription] ([ID])
GO

GO
ALTER TABLE dbo.Account ADD
	Disabled bit NULL
GO


GO
ALTER TABLE dbo.Account ADD
	Hidden bit NULL
GO

GO
EXECUTE sp_rename N'dbo.Account.Hidden', N'Tmp_Approved', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.Account.Tmp_Approved', N'Approved', 'COLUMN' 
GO

GO
ALTER TABLE dbo.Lesson ADD
	Hidden bit NULL
GO