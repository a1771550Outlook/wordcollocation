USE [WordCollocationTest]
GO

/****** Object:  Table [dbo].[Collocations]    Script Date: 09/02/2014 22:29:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Collocations](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[posId] [smallint] NOT NULL,
	[colPosId] [smallint] NOT NULL,
	[wordId] [bigint] NOT NULL,
	[colWordId] [bigint] NOT NULL,
	[CollocationPattern] [int] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_dbo.Collocations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Collocations]  WITH CHECK ADD  CONSTRAINT [FK_Collocations_ColPoss] FOREIGN KEY([colPosId])
REFERENCES [dbo].[ColPoss] ([Id])
GO

ALTER TABLE [dbo].[Collocations] CHECK CONSTRAINT [FK_Collocations_ColPoss]
GO

ALTER TABLE [dbo].[Collocations]  WITH CHECK ADD  CONSTRAINT [FK_Collocations_ColWords] FOREIGN KEY([colWordId])
REFERENCES [dbo].[ColWords] ([Id])
GO

ALTER TABLE [dbo].[Collocations] CHECK CONSTRAINT [FK_Collocations_ColWords]
GO

ALTER TABLE [dbo].[Collocations]  WITH CHECK ADD  CONSTRAINT [FK_Collocations_Poss] FOREIGN KEY([posId])
REFERENCES [dbo].[Poss] ([Id])
GO

ALTER TABLE [dbo].[Collocations] CHECK CONSTRAINT [FK_Collocations_Poss]
GO

ALTER TABLE [dbo].[Collocations]  WITH CHECK ADD  CONSTRAINT [FK_Collocations_Words] FOREIGN KEY([wordId])
REFERENCES [dbo].[Words] ([Id])
GO

ALTER TABLE [dbo].[Collocations] CHECK CONSTRAINT [FK_Collocations_Words]
GO

