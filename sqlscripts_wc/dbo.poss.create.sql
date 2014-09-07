USE [WordCollocationTest]
GO

/****** Object:  Table [dbo].[Poss]    Script Date: 09/02/2014 22:28:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Poss](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Entry] [nvarchar](20) NOT NULL,
	[EntryZht] [nvarchar](30) NOT NULL,
	[EntryZhs] [nvarchar](30) NOT NULL,
	[EntryJap] [nvarchar](30) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_dbo.Poss] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

