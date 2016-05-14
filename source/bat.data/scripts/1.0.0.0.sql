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

