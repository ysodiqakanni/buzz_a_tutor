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

