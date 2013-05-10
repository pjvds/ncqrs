IF EXISTS(SELECT * FROM sysobjects WHERE xtype = 'UQ' AND name = 'UQ_Events_Id' AND uid = SCHEMA_ID(SCHEMA_NAME()))
	BEGIN
		PRINT 'The UQ_Events_Id unique-key constraint already exists.'
	END
ELSE
	BEGIN
		ALTER TABLE [Events] ADD CONSTRAINT [UQ_Events_Id] UNIQUE ([Id])

		PRINT 'The UQ_Events_Id unique-key constraint was created.'
	END

IF EXISTS(SELECT * FROM sysobjects WHERE xtype = 'F' AND name = 'FK_Events_EventSources' AND uid = SCHEMA_ID(SCHEMA_NAME()))
	BEGIN
		PRINT 'The FK_Events_EventSources foreign-key constraint already exists.'
	END
ELSE
	BEGIN
		ALTER TABLE [Events] WITH CHECK ADD CONSTRAINT [FK_Events_EventSources] FOREIGN KEY([EventSourceId])
			REFERENCES [EventSources] ([Id])

		ALTER TABLE [Events] CHECK CONSTRAINT [FK_Events_EventSources]

		PRINT 'The FK_Events_EventSources foreign-key constraint was created.'
	END

IF EXISTS(SELECT * FROM sysobjects WHERE xtype = 'F' AND name = 'FK_Snapshots_EventSources' AND uid = SCHEMA_ID(SCHEMA_NAME()))
	BEGIN
		PRINT 'The FK_Snapshots_EventSources foreign-key constraint already exists.'
	END
ELSE
	BEGIN
		ALTER TABLE [Snapshots] WITH CHECK ADD CONSTRAINT [FK_Snapshots_EventSources] FOREIGN KEY([EventSourceId])
			REFERENCES [EventSources] ([Id])

		ALTER TABLE [Snapshots] CHECK CONSTRAINT [FK_Snapshots_EventSources]

		PRINT 'The FK_Snapshots_EventSources foreign-key constraint was created.'
	END

IF EXISTS(SELECT * FROM sysobjects WHERE xtype = 'F' AND name = 'FK_PipelineState_Events' AND uid = SCHEMA_ID(SCHEMA_NAME()))
	BEGIN
		PRINT 'The FK_PipelineState_Events foreign-key constraint already exists.'
	END
ELSE
	BEGIN
		ALTER TABLE [PipelineState] WITH CHECK ADD CONSTRAINT [FK_PipelineState_Events] FOREIGN KEY([LastProcessedEventId])
			REFERENCES [Events] ([Id])

		ALTER TABLE [PipelineState] CHECK CONSTRAINT [FK_PipelineState_Events]

		PRINT 'The FK_PipelineState_Events foreign-key constraint was created.'
	END