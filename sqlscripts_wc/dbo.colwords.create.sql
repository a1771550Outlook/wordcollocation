USE [WordCollocationTest]
GO

/****** Object:  Table [dbo].[ColWords]    Script Date: 09/02/2014 22:29:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ColWords](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Entry] [nvarchar](50) NOT NULL,
	[EntryZht] [nvarchar](200) NOT NULL,
	[EntryZhs] [nvarchar](200) NULL,
	[EntryJap] [nvarchar](200) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_dbo.ColWords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

