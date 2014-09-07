USE [WordCollocationContext]
GO

/****** Object:  Table [dbo].[Prepositions]    Script Date: 2014-05-02 11:49:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Prepositions](
	[prepId] [nvarchar](20) NOT NULL,
	[Entry] [nvarchar](30) NOT NULL,
	[EntryChi] [nvarchar](50) NULL,
	[EntryJap] [nvarchar](50) NULL,
 CONSTRAINT [PK_Prepositions] PRIMARY KEY CLUSTERED 
(
	[prepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

