USE [WordCollocationTest]
GO

/****** Object:  Table [dbo].[WcExamples]    Script Date: 09/02/2014 22:29:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WcExamples](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Entry] [nvarchar](1000) NOT NULL,
	[EntryZht] [nvarchar](1000) NULL,
	[EntryZhs] [nvarchar](1000) NULL,
	[EntryJap] [nvarchar](1000) NULL,
	[Source] [nvarchar](200) NULL,
	[RemarkZht] [nvarchar](200) NULL,
	[RemarkZhs] [nvarchar](200) NULL,
	[RemarkJap] [nvarchar](200) NULL,
	[CollocationId] [bigint] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_dbo.WcExamples] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WcExamples]  WITH CHECK ADD  CONSTRAINT [FK_WcExamples_Collocations] FOREIGN KEY([CollocationId])
REFERENCES [dbo].[Collocations] ([Id])
GO

ALTER TABLE [dbo].[WcExamples] CHECK CONSTRAINT [FK_WcExamples_Collocations]
GO

